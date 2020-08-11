using Sharer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TensileTesterSharer.ViewModel;



namespace TensileTesterSharer
{
   
    public class DataGenerator
    {
       
        public int DataCount = 50000;
        private int RateOfData = 5;
        private ObservableCollection<Data> Data;
        private Random randomNumber;
        int myindex = 0;
        DispatcherTimer timer;
        
        public ObservableCollection<Data> DynamicData { get; set; }
        
        public DataGenerator()
        {


            randomNumber = new Random();
            DynamicData = new ObservableCollection<Data>();
            Data = new ObservableCollection<Data>();
            Data = ZeroData();
            RunTmr();          
        }

        public void RunTmr()
        {
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Start();
        }
        

        public void AddData()
        {
            DateTime date = new DateTime(2009, 1, 1);
            date = date.Add(TimeSpan.FromSeconds(1));
            double force = SharedVariables.LoadcellScaled;
            double enc = SharedVariables.EncoderScaled;
            double ana = SharedVariables.AnaTest;

            DynamicData.Add(new Data(date, force, enc, ana));
            
                
         }
        
        public ObservableCollection<Data> ZeroData()
        {
            ObservableCollection<Data> datas = new ObservableCollection<Data>();

            DateTime date = new DateTime(2009, 1, 1);
            double force = 0;
            double enc = 0;
            double ana = 0;
           
                datas.Add(new Data(date, force, enc, ana));
                               
            return datas;
        }
        
        private void timer_Tick(object sender, EventArgs e)
        {
            if(SharedVariables.MOETestStartred == true || SharedVariables.IBTestStartred == true)
            {
                SharedVariables.ForceSample2 = SharedVariables.ForceSample1;
                SharedVariables.ForceSample1 = SharedVariables.AnaTest;
                if((SharedVariables.ForceSample2-SharedVariables.ForceSample1)> SharedVariables.BreakForce)
                {
                    SharedVariables.TestComplete = true;
                    
                }
                if(SharedVariables.AnaTest > SharedVariables.MaxForce)
                {
                    SharedVariables.MaxForce = SharedVariables.AnaTest;
                }
                AddData();
            }
            
        }
        
    }
    


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public float LoadcellScaled = 0;
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
        private bool ReturnSelected = false;
        private int isWorking;
        private System.Threading.Timer ReadArduinoTmr;
        public SharerConnection connection;
        DispatcherTimer UpdateUItmr;
        

        public MainWindow()
        {
            InitializeComponent();

            SharedVariables.ReturnOffset = Settings.Default.RetOffset;
            SharedVariables.BreakForce = Settings.Default.BrkForce;
            sldManSpd.Value = 0;
            sldAutoSpd.Value = 0;
            SharedVariables.TestComplete = false;
            string[] ports = SerialPort.GetPortNames();
            foreach (string comport in ports)
            {
                cmb_Port.Items.Add(comport);
            }
           
            connect();
           
        }

        public void connect()
        {
            try
            {
                connection = new SharerConnection("COM23", 115200);
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
                ReadArduinoTmr = new System.Threading.Timer(ReadArduino, null, 500, 200);
                var CalFactorTemp = connection.ReadVariable("calibration_factor");
                txt_CalFactor.Value = Convert.ToInt32(CalFactorTemp.Value);

                UpdateUItmr = new DispatcherTimer();
                UpdateUItmr.Tick += UpdateUItmr_Tick;
                UpdateUItmr.Interval = new TimeSpan(0, 0, 0, 0, 200);
                UpdateUItmr.Start();

            }
            else
            {
                MessageBox.Show("Cannot Connect, Please check Cabling and Retry");
            }
            
        }

        
        public void ReadArduino(object state)
        {
           

            if (connection.Connected)
            {

                if ((Interlocked.Increment(ref isWorking) == 1))
                {
                    _ = this.Dispatcher.BeginInvoke((System.Action)(() =>
                    {

                        try
                        {
                        var values = connection.ReadVariables(new string[] { "encoder", "Loadcell_Mass", "ana", "Srdy", "Szpd" });
                        
                            SharedVariables.EncoderRaw = Convert.ToDouble(values[0].Value);
                            SharedVariables.LoadcellScaled = Convert.ToDouble(values[1].Value);
                            SharedVariables.AnaTest = Convert.ToInt16(values[2].Value);
                            SharedVariables.Servo_Rdy = Convert.ToBoolean(values[3].Value);
                            SharedVariables.Servo_ZSpd = Convert.ToBoolean(values[4].Value);
                        }

                        catch (NullReferenceException)
                        {
                            MessageBox.Show("Null Ref");
                        }

                        catch (Exception ex)
                        {
                            
                            //MessageBox.Show(ex.ToString());
                        }

                        finally
                        {
                            Interlocked.Decrement(ref isWorking);
                        }
                    }));

                }

                else
                {
                    Interlocked.Decrement(ref isWorking);
                }

            }

            else
            {
                ReadArduinoTmr.Dispose();
                MessageBox.Show("Connection Lost!! Resolve issue and reconnect");
            }
            
        }

        private void UpdateUItmr_Tick(object sender, EventArgs e)
        {

            txt_Force.Value = SharedVariables.AnaTest;
            SharedVariables.EncoderScaled = ((SharedVariables.EncoderRaw / 300.25));
            txt_Dist.Value = SharedVariables.EncoderScaled;
            //txt_Ana.Value = SharedVariables.AnaTest;
            txt_SR.Text = SharedVariables.Servo_Rdy.ToString();
            txt_SZ.Text = SharedVariables.Servo_ZSpd.ToString();
            txtMaxForce.Value = SharedVariables.MaxForce;
            if(SharedVariables.TestComplete == true)
            {
                TestComplete();
            }
            if(ReturnSelected == true && MOETest==true)
            {
                { 
                if (SharedVariables.EncoderScaled > SharedVariables.ReturnOffset)
                {
                    SlideUP();
                }
                else if (SharedVariables.EncoderScaled <= SharedVariables.ReturnOffset)
                {
                    SlideUpStop();
                }
                   
                    ReturnSelected = false;
                }
            }
            if (ReturnSelected == true && IBTest == true)
            {
                {
                    if (SharedVariables.EncoderScaled > SharedVariables.ReturnOffset)
                    {
                        SlideDown();
                    }
                    else if (SharedVariables.EncoderScaled <= SharedVariables.ReturnOffset)
                    {
                        SlideDownStop();
                    }

                    ReturnSelected = false;
                }
            }

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
            }
        }

        private void btn_Down_Unchecked(object sender, RoutedEventArgs e)
        {
            SlideDownStop();
            btn_Down.Content = "Down";
            btn_Down.Background = new SolidColorBrush(Colors.Green);
            ManDown = false;
        }
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            int calfactorTemp = (int)txt_CalFactor.Value;
            connection.WriteVariable("calibration_factor", calfactorTemp);
        }

        private void Tare_Click(object sender, RoutedEventArgs e)
        {
            connection.Call("Tare");
        }

        private void btn_connect_Click(object sender, RoutedEventArgs e)
        {
            if (!connection.Connected)
            {
                connect();
            }
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            //TestChart.Series.
            ReturnSelected = true;
           
        }

        private void sldManSpd_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ManSpeed = (int)sldManSpd.Value;
            WriteSpeedRef();

        }

        private void sldAutoSpd_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AutoSpeedTest = (int)sldAutoSpd.Value;
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
           
            else if (ManSelect == false && MOETest == false && IBTest == false)
            {
                MessageBox.Show("Please Select Test Type");
                btnAutoStart.IsChecked = false;
                btnAutoStart.Background = new SolidColorBrush(Colors.Green);
            }
            else if ((ManSelect == false) && (MOETest == true || IBTest == true))
                {
                btnAutoStart.Content = "Test Running";
                btnAutoStart.Background = new SolidColorBrush(Colors.Red);
                Autostart = true;
                btnMOE.IsEnabled = false;
                btnIB.IsEnabled = false;
                btn_Man.IsEnabled = false;
                btn_Up.IsEnabled = false;
                btn_Down.IsEnabled = false;
                SharedVariables.TestComplete = false;
                SharedVariables.MaxForce = 0;
                connection.Call("Tare");
                connection.Call("ZeroEnc");
                //TestChart.IsEnabled = true;
                if(MOETest== true)
                {
                    SharedVariables.MOETestStartred = true;
                    SharedVariables.IBTestStartred = false;
                    SlideDown();
                }
                else if (IBTest == true)
                {
                    SharedVariables.IBTestStartred = true;
                    SharedVariables.MOETestStartred = false;
                    SlideUP();
                }
               
            }
           
        }

        private void btnAutoStart_Unchecked(object sender, RoutedEventArgs e)
        {

            TestComplete();
                      
        }

        public void TestComplete()
        {
            btnAutoStart.Content = "Start";
            btnAutoStart.Background = new SolidColorBrush(Colors.Green);
            Autostart = false;
            btnMOE.IsEnabled = true;
            btnIB.IsEnabled = true;
            btn_Man.IsEnabled = true;
            btn_Up.IsEnabled = true;
            btn_Down.IsEnabled = true;
            if (SharedVariables.MOETestStartred == true)
            {
                SharedVariables.MOETestStartred = false;
                SlideDownStop();
            }
            else if (SharedVariables.IBTestStartred == true)
            {
                SharedVariables.IBTestStartred = false;
                SlideUpStop();
            }
        }
        private void btnIB_Checked(object sender, RoutedEventArgs e)
        {
            MOETest = false;
            btnMOE.Background = new SolidColorBrush(Colors.Green);
            IBTest = true;
            btnIB.Background = new SolidColorBrush(Colors.Red);
        }

        private void btnIB_Unchecked(object sender, RoutedEventArgs e)
        {
            IBTest = false;
            btnIB.Background = new SolidColorBrush(Colors.Green);
        }

        private void btnMOE_Checked(object sender, RoutedEventArgs e)
        {
            IBTest = false;
            btnIB.Background = new SolidColorBrush(Colors.Green);
            MOETest = true;
            btnMOE.Background = new SolidColorBrush(Colors.Red);
        }

        private void btnMOE_Unchecked(object sender, RoutedEventArgs e)
        {
            MOETest = false;
            btnMOE.Background = new SolidColorBrush(Colors.Green);
        }
        #endregion

        #region Arduino
        private void WriteSpeedRef()
        {
            int SpeedRef;

            if (ManSelect == true)
            {
                SpeedRef = (int)(ManSpeed * 2.5);
            }
            else
            {
                SpeedRef = (int)(AutoSpeedTest * 2.5);
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

        private void txt_CalFactor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                connection.WriteVariable("calibration_factor", txt_CalFactor.Value);
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot Write");
            }

        }
        #endregion

        private void TestSeup_Click(object sender, RoutedEventArgs e)
        {
            TestSetup testSetup = new TestSetup();
            SharedVariables sharedVariables = new SharedVariables();
            bool? Result1 = testSetup.ShowDialog();
        }

        private void MenuItem_About_Click()
        {

        }

        private void MenuItem_Close_Click(object sender, RoutedEventArgs e)
        {
            SlideDownStop();
            SlideUpStop();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            Application.Current.Shutdown();
            Close();
        }


    }
}
