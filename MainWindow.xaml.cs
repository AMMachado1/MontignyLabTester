using Sharer;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TensileTesterSharer.Properties;
using SUT.PrintEngine.Utils;
  

namespace TensileTesterSharer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public float LoadcellScaled;
        public int ManSpeed = 0;
        public int AutoSpeedTest = 0;
        public int AutoSpeedApproach = 0;
        public double ReturnOffset;
        public bool ManSelect = true;
        public bool ManUP = false;
        public bool ManDown = false;
        public bool Autostart = false;
        public bool IBTest = false;
        public bool MOETest = false;
        public bool FaceTest = false;
        public bool ScrewTest = false;
        public int TestStepCount = 0;
        public bool TestCompleteOneShot = false;
        public string TestSelected;
        private bool ReturnSelected = false;
        public bool exists = false;
        public SharerConnection connection;
        DispatcherTimer UpdateUItmr;
        DispatcherTimer Arduino;
        DispatcherTimer SampleBreak;

       // public ObservableCollection<Data> DynamicData { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            SharedVariables.ReturnOffset = Settings.Default.RetOffset;
            SharedVariables.BreakForce = Settings.Default.BrkForce;
            SharedVariables.EncFac = Settings.Default.EncFac;
            SharedVariables.ForceMpaSample1 = 0;
           
            sldManSpd.Value = 0;
           
            SharedVariables.TestComplete = false;

            Connect connectWin = new Connect();
            bool? Result1 = connectWin.ShowDialog();
            connect();
            
        }

        public void connect()
        {
            try
            {
                connection = new SharerConnection(SharedVariables.ComPort, 38400);
                //connection = new SharerConnection(cmb_Port.Text, 115200);
                connection.Connect();
                connection.RefreshFunctions();
            }
            catch (Exception)
            {
                MessageBox.Show("Please Check Connection!!");

            }


            if (connection.Connected)
            {
                UpdateUItmr = new DispatcherTimer();
                UpdateUItmr.Interval = new TimeSpan(0, 0, 0, 0, 100);
                UpdateUItmr.Tick += UpdateUItmr_Tick;
                UpdateUItmr.Start();
                Arduino = new DispatcherTimer();
                Arduino.Interval = new TimeSpan(0, 0, 0, 0, 100);
                Arduino.Tick += Arduino_Tick;
                Arduino.Start();
                SampleBreak = new DispatcherTimer();
                SampleBreak.Interval = new TimeSpan(0, 0, 0, 0, 1000);
                SampleBreak.Tick += SampleBreak_Tick;
                SampleBreak.Start();

            }
            else
            {
                MessageBox.Show("Cannot Connect, Please check Cabling and Retry");
            }

        }

        private void SampleBreak_Tick(object sender, EventArgs e)
        {

            if (Autostart == true && SharedVariables.TestIBMpa > 0.1)
            {
                //SharedVariables.ForceMpaSample2 = SharedVariables.ForceMpaSample1;
                SharedVariables.ForceMpaSample1 = SharedVariables.TestIBMpa;
                
            }
        }
        private void Arduino_Tick(object sender, EventArgs e)
        {
            try
            {
                var values = connection.ReadVariables(new string[] { "encoder", "Loadcell_Mass", "Srdy",});
                SharedVariables.EncoderRaw = Convert.ToDouble(values[0].Value);
                SharedVariables.LoadcellScaled = Convert.ToDouble(values[1].Value);
                SharedVariables.Servo_Rdy = Convert.ToBoolean(values[2].Value);
               
            }

            catch (Exception ex)
            {
                MessageBox.Show("Connection Lost, Please check and Press Connect" + ex);
                connection.Disconnect();
                Arduino.Stop();
            }

        }

        public void UpdateUItmr_Tick(object sender, EventArgs e)
        {
            SharedVariables.EncoderScaled = SharedVariables.EncoderRaw / SharedVariables.EncFac;
            txt_Dist.Value = SharedVariables.EncoderScaled;
            SharedVariables.TestMpa = ((SharedVariables.LoadcellScaled * 0.098)/25);
            SharedVariables.TestIBMpa = (SharedVariables.LoadcellScaled * 0.0098);
            txt_Kg.Value = SharedVariables.LoadcellScaled;
            if (SharedVariables.TestMpa > SharedVariables.MaxForce)
            {
                SharedVariables.MaxForce = SharedVariables.TestMpa;
            }

            if (SharedVariables.TestIBMpa > SharedVariables.MaxKn)
            {
                SharedVariables.MaxKn = SharedVariables.TestIBMpa;
            }

            if (SharedVariables.ForceMpaSample1 > (SharedVariables.TestIBMpa + SharedVariables.BreakForce))// - SharedVariables.BreakForce)) // > SharedVariables.BreakForce)
            {
                TestComplete();

            }

            if (SharedVariables.TestStarted == true)
            {
                txt_Force.Value = SharedVariables.TestIBMpa;
                txtMaxForce.Value = SharedVariables.TestMpa;
            }
            else if (SharedVariables.TestStarted == false)
            {
                txt_Force.Value = SharedVariables.MaxKn;
                txtMaxForce.Value = SharedVariables.MaxForce;
            }
            if (SharedVariables.TestComplete == true)
            {
                TestCompleteOneShot = true;
                TestComplete();
            }
            if (ReturnSelected == true)
            {

                if (SharedVariables.EncoderScaled > SharedVariables.ReturnOffset)
                {
                    SlideUP();
                }
                else if (SharedVariables.EncoderScaled <= SharedVariables.ReturnOffset)
                {
                    SlideUpStop();
                    ReturnSelected = false;
                    btnReturn.IsChecked = false;
                }
            }

            if(SharedVariables.Servo_Rdy == true)
            {
                SlideDownStop();
                SlideUpStop();
                MessageBox.Show("Servo Drive Fault!!");
            }
            if(SharedVariables.CalWinTare == true)
            {
                Tare();
            }
            if (SharedVariables.CalWinCal == true)
            {
                Calibrate();
            }

        }

        public void Chart()
        {
            TestChart.Series.Clear();
            FastLineSeries fastline = new FastLineSeries();
            fastline.ItemsSource = (new DataGenerator()).DynamicData;
            fastline.XBindingPath = "Enc";
            fastline.YBindingPath = "Force";
            fastline.StrokeThickness = 2;
            TestChart.Series.Add(fastline);            
            
        }
    
           #region Buttons

        private void btn_Man_Checked(object sender, RoutedEventArgs e)
        {
            btn_Man.Content = "In Manual";
            ManSelect = true;
            btn_Man.Background = new SolidColorBrush(Colors.Red);
        }

        private void btn_Man_Unchecked(object sender, RoutedEventArgs e)
        {
            btn_Man.Content = "In Auto";
            ManSelect = false;
            btn_Man.Background = new SolidColorBrush(Colors.Green);
            SlideUpStop();
            ManUP = false;
            btn_Up.Content = "Up";
            SlideDownStop();
            btn_Down.Content = "Down";
            ManDown = false;
        }

        private void btn_Up_Checked(object sender, RoutedEventArgs e)
        {
            if (ManUP == false && ManSelect == true && ManDown == false)
            {
                SlideUP();
                btn_Up.Content = "Stop";
                btn_Up.Background = new SolidColorBrush(Colors.Red);
                ManUP = true;
                btn_Down.IsChecked = false;
            }
        }

        private void btn_Up_Unchecked(object sender, RoutedEventArgs e)
        {
            SlideUpStop();
            btn_Up.Content = "Up";
            btn_Up.Background = new SolidColorBrush(Colors.Green);
            ManUP = false;
        }

        private void btn_Down_Checked(object sender, RoutedEventArgs e)
        {
            if (ManDown == false && ManSelect == true && ManUP == false)
            {
                SlideDown();
                btn_Down.Content = "Stop";
                btn_Down.Background = new SolidColorBrush(Colors.Red);
                ManDown = true;
                btn_Up.IsChecked = false;
            }
        }

        private void btn_Down_Unchecked(object sender, RoutedEventArgs e)
        {
            SlideDownStop();
            btn_Down.Content = "Down";
            btn_Down.Background = new SolidColorBrush(Colors.Green);
            ManDown = false;
        }
       
        private void Calibrate()
        {
            try
            {
                connection.WriteVariable("Cal_Weight", SharedVariables.LoadFac);
                connection.Call("Calibrate");
                SharedVariables.CalWinCal = false;
            }
            catch (Exception i)
            {

                MessageBox.Show("Could not Write" + i);
            }

        }
        private void btnTare_Click(object sender, RoutedEventArgs e)
        {

            Tare();
            SharedVariables.CalWinTare = false;
        }

        private void Tare()
        {
            try
            {
                connection.Call("Tare");
            }
            catch (Exception i)
            {

                MessageBox.Show("Could not Write" + i);
            }
        }

        private void btnZero_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Call("ZeroEnc");
            }
            catch (Exception i)
            {

                MessageBox.Show("Could not Write" + i);
            }
            
        }

        private void btn_connect_Click(object sender, RoutedEventArgs e)
        {
            if (!connection.Connected)
            {
                connect();
            }
        }

        private void btnReturn_Checked(object sender, RoutedEventArgs e)
        {
            if (Autostart == false)
            {
                ReturnSelected = true;
                btnReturn.Background = new SolidColorBrush(Colors.Red);
            }

            else
            {
                MessageBox.Show("Please Stop Test First");
                btnReturn.IsChecked = false;
            }
        }

        private void btnReturn_Unchecked(object sender, RoutedEventArgs e)
        {
            ReturnSelected = false;
            btnReturn.Background = new SolidColorBrush(Colors.Green);
        }

        private void sldManSpd_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ManSpeed = (int)sldManSpd.Value;
            WriteSpeedRef();

        }

       
        private void btnAutoStart_Checked(object sender, RoutedEventArgs e)
        {
            if (ManSelect == true)
            {
                MessageBox.Show("In Manual");
                btnAutoStart.IsChecked = false;
                btnAutoStart.Background = new SolidColorBrush(Colors.Green);
            }

            else if (ManSelect == false && MOETest == false && IBTest == false && FaceTest == false && ScrewTest == false)
            {
                MessageBox.Show("Please Select Test Type");
                btnAutoStart.IsChecked = false;
                btnAutoStart.Background = new SolidColorBrush(Colors.Green);
            }
            else if ((ManSelect == false) && (MOETest == true || IBTest == true || FaceTest == true || ScrewTest == true))
            {
                if (TestStepCount == 0)
                {
                    TestInfo testInfoWin = new TestInfo();
                    bool? Result = testInfoWin.ShowDialog();
                }
                
                btnAutoStart.Content = "Test Running";
                btnAutoStart.Background = new SolidColorBrush(Colors.Red);
                ManSelect = false;
                Autostart = true;
                btnMOE.IsEnabled = false;
                btnIB.IsEnabled = false;
                btnFS.IsEnabled = false;
                btnSH.IsEnabled = false;
                btn_Man.IsEnabled = false;
                btn_Up.IsEnabled = false;
                btn_Down.IsEnabled = false;
                btnReturn.IsEnabled = false;
                TestCompleteOneShot = true;
                SharedVariables.TestComplete = false;
                SharedVariables.TestStarted = true;
                SharedVariables.MaxForce = 0;
                SharedVariables.MaxKn = 0;
                SharedVariables.ResetChart = true;
                SharedVariables.ForceMpaSample1 = 0;
                try
                {
                    connection.Call("Tare");
                    connection.Call("ZeroEnc");
                    WriteSpeedRef();
                }
                catch (Exception)
                {

                    MessageBox.Show("Connection Lost, Please Check");
                }
                
                Chart();
                SharedVariables.MOETestStarted = true;
                SlideDown();
            }
        }

        private void btnAutoStart_Unchecked(object sender, RoutedEventArgs e)
        {
           
                TestComplete();
            
        }

        public void TestComplete()
        {
            if (TestCompleteOneShot == true)
            {
                btnAutoStart.Content = "Start";
                btnAutoStart.Background = new SolidColorBrush(Colors.Green);
                Autostart = false;
                btnMOE.IsEnabled = true;
                btnIB.IsEnabled = true;
                btnFS.IsEnabled = true;
                btnSH.IsEnabled = true;
                btn_Man.IsEnabled = true;
                btn_Up.IsEnabled = true;
                btn_Down.IsEnabled = true;
                btnReturn.IsEnabled = true;
                TestCompleteOneShot = false;
                SharedVariables.TestStarted = false;
                TestStepCount = ++TestStepCount;
                lblTestStep.Content = (TestSelected + "     " + TestStepCount.ToString());
                if (IBTest == true)
                {
                    if (TestStepCount == 1)
                    {
                        SharedVariables.IB1T = SharedVariables.MaxKn;
                    }
                    if (TestStepCount == 2)
                    {
                        SharedVariables.IB2B = SharedVariables.MaxKn;
                    }
                    if (TestStepCount == 3)
                    {
                        SharedVariables.IB3T = SharedVariables.MaxKn;
                    }
                    if (TestStepCount == 4)
                    {
                        SharedVariables.IB4B = SharedVariables.MaxKn;
                    }
                    if (TestStepCount == 5)
                    {
                        SharedVariables.IB5T = SharedVariables.MaxKn;
                    }
                    if (TestStepCount == 6)
                    {
                        SharedVariables.IB6B = SharedVariables.MaxKn;
                    }
                    if (TestStepCount == 7)
                    {
                        SharedVariables.IB7T = SharedVariables.MaxKn;
                    }
                    if (TestStepCount == 8)
                    {
                        SharedVariables.IB8B = SharedVariables.MaxKn;
                        lblTestStep.Content = ("Test Complete");
                        IBTest = false;
                        btnIB.IsChecked = false;
                    }

                }
                if (MOETest == true)
                {
                    if (TestStepCount == 1)
                    {
                        SharedVariables.IB1Mpa = SharedVariables.MaxForce;
                    }
                    if (TestStepCount == 2)
                    {
                        SharedVariables.IB2Mpa = SharedVariables.MaxForce;
                    }
                    if (TestStepCount == 3)
                    {
                        SharedVariables.IB3Mpa = SharedVariables.MaxForce;
                    }
                    if (TestStepCount == 4)
                    {
                        SharedVariables.IB4Mpa = SharedVariables.MaxForce;
                    }
                    if (TestStepCount == 5)
                    {
                        SharedVariables.IB5Mpa = SharedVariables.MaxForce;
                    }
                    if (TestStepCount == 6)
                    {
                        SharedVariables.IB6Mpa = SharedVariables.MaxForce;
                    }
                    if (TestStepCount == 7)
                    {
                        SharedVariables.IB7Mpa = SharedVariables.MaxForce;
                    }
                    if (TestStepCount == 8)
                    {
                        SharedVariables.IB8Mpa = SharedVariables.MaxForce;
                        lblTestStep.Content = ("Test Complete");
                        MOETest = false;
                        btnMOE.IsChecked = false;
                    }

                }

                if (FaceTest == true)
                {
                    if (TestStepCount == 1)
                    {
                        SharedVariables.Shf1 = SharedVariables.MaxKn;
                    }
                    if (TestStepCount == 2)
                    {
                        SharedVariables.Shf2 = SharedVariables.MaxKn;
                    }
                    if (TestStepCount == 3)
                    {
                        SharedVariables.Shf3 = SharedVariables.MaxKn;
                    }
                    if (TestStepCount == 4)
                    {
                        SharedVariables.Shf4 = SharedVariables.MaxKn;
                        lblTestStep.Content = ("Test Complete");
                        FaceTest = false;
                        btnFS.IsChecked = false;
                    }

                }
                if (ScrewTest == true)
                {
                    if (TestStepCount == 1)
                    {
                        SharedVariables.She1 = SharedVariables.MaxKn;
                    }
                    if (TestStepCount == 2)
                    {
                        SharedVariables.She1 = SharedVariables.MaxKn;
                    }
                    if (TestStepCount == 3)
                    {
                        SharedVariables.She1 = SharedVariables.MaxKn;
                    }
                    if (TestStepCount == 4)
                    {
                        SharedVariables.She1 = SharedVariables.MaxKn;
                        lblTestStep.Content = ("Test Complete");
                        ScrewTest = false;
                        btnSH.IsChecked = false;
                    }
                }
                SlideDownStop();
              
            }

        }
        private void btnIB_Checked(object sender, RoutedEventArgs e)
        {
            MOETest = false;
            FaceTest = false;
            ScrewTest = false;
            btnMOE.Background = new SolidColorBrush(Colors.Green);
            IBTest = true;
            btnIB.Background = new SolidColorBrush(Colors.Red);
            btnMOE.IsChecked = false;
            TestSelected = ("IB");
            lblTestStep.Content = ("IB");
            TestStepCount = 0;
        }

        private void btnIB_Unchecked(object sender, RoutedEventArgs e)
        {
            IBTest = false;
            btnIB.Background = new SolidColorBrush(Colors.Green);
        }

        private void btnMOE_Checked(object sender, RoutedEventArgs e)
        {
            IBTest = false;
            FaceTest = false;
            ScrewTest = false;
            btnIB.Background = new SolidColorBrush(Colors.Green);
            MOETest = true;
            btnMOE.Background = new SolidColorBrush(Colors.Red);
            btnIB.IsChecked = false;
            TestSelected = ("MOR");
            lblTestStep.Content = ("MOR");
            TestStepCount = 0;
        }

        private void btnMOE_Unchecked(object sender, RoutedEventArgs e)
        {
            MOETest = false;
            btnMOE.Background = new SolidColorBrush(Colors.Green);
        }

        private void btnSH_Checked(object sender, RoutedEventArgs e)
        {
            ScrewTest = true;
            MOETest = false;
            IBTest = false;
            FaceTest = false;
            btnSH.Background = new SolidColorBrush(Colors.Red);
            TestSelected = ("SCREW");
            lblTestStep.Content = ("SCREW");
            TestStepCount = 0;
        }

        private void btnSH_Unchecked(object sender, RoutedEventArgs e)
        {
            ScrewTest = false;
            btnSH.Background = new SolidColorBrush(Colors.Green);
        }

        private void btnFS_Checked(object sender, RoutedEventArgs e)
        {
            FaceTest = true;
            MOETest = false;
            IBTest = false;
            ScrewTest = false;
            btnFS.Background = new SolidColorBrush(Colors.Red);
            TestSelected = ("FACE");
            lblTestStep.Content = ("FACE");
            TestStepCount = 0;
        }

        private void btnFS_Unchecked(object sender, RoutedEventArgs e)
        {
            FaceTest = false;
            btnFS.Background = new SolidColorBrush(Colors.Green);
        }

        private void TestSeup_Click(object sender, RoutedEventArgs e)
        {
            TestSetup testSetup = new TestSetup();
            SharedVariables sharedVariables = new SharedVariables();
            bool? Result1 = testSetup.ShowDialog();
        }

        private void MenuItem_Close_Click(object sender, RoutedEventArgs e)
        {
            SlideDownStop();
            SlideUpStop();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            Application.Current.Shutdown();
            Close();
        }

        private void btnReport_Checked(object sender, RoutedEventArgs e)
        {
            Report report = new Report();
            bool? Result1 = report.ShowDialog();
        }

        private void btnChartPrint_Click(object sender, RoutedEventArgs e)
        {
            TestChart.Print();
        }

        

        private void Calibration_Click(object sender, RoutedEventArgs e)
        {
            Calibration calibration = new Calibration();
            bool? Result = calibration.ShowDialog();
        }

        private void MenuItem_Cal_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            bool? Result1 = about.ShowDialog();
        }


        private void btnTestReset_Click(object sender, RoutedEventArgs e)
        {
            TestStepCount = 0;
        }

        private void MenuItem_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!connection.Connected)
            {
                Connect connectWin = new Connect();
                bool? Result1 = connectWin.ShowDialog();
                connect();
            }
        }
        #endregion

        #region Arduino
        private void WriteSpeedRef()
        {
            int SpeedRef;

            if (ManSelect == true)
            {
                SpeedRef = (int)(ManSpeed*2.5);
            }
            else
            {
                SpeedRef = (int)(SharedVariables.AutoSpeed);
            }

            try
            {

                connection.WriteVariable("SpeedRef", SpeedRef);

            }
            catch (Exception)
            {
                MessageBox.Show("Connection Lost!! Resolve issue and reconnect");

            }

        }

        private void SlideUP()
        {
            try
            {
                var Up = connection.Call("SlideUp");
            }
            catch (Exception)
            {

                MessageBox.Show("Cannot Write");
            }

        }

        private void SlideUpStop()
        {
            try
            {
                var Up = connection.Call("SlideUpStop");
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot Write");
            }

        }

        private void SlideDown()
        {
            try
            {
                var Down = connection.Call("SlideDown");
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot Write");
            }

        }

        private void SlideDownStop()
        {
            try
            {
                var Down = connection.Call("SlideDownStop");
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot Write");
            }

        }

        #endregion

    }
}