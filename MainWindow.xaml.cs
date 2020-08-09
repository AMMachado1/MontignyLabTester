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
using TensileTesterSharer.ViewModel;


namespace TensileTesterSharer
{
    public class SharedVariables
    {
        public static double EncoderScaled { get; set; }
        public static double LoadcellScaled { get; set; }
        public static double EncoderRaw;
        public static double LoadcellRaw;
        public static int AnaTest;
        public static double LoadTest;
        public static bool Servo_Rdy;
        public static bool Servo_ZSpd;
        
    }
    
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
            Data = GenerateData();
           
            // LoadData();

            timer = new DispatcherTimer();
                timer.Tick += timer_Tick;
                timer.Interval = new TimeSpan(0, 0, 0, 0,10);
                timer.Start();
           
        }
        

        public void AddData()
        {
            DateTime date = new DateTime(2009, 1, 1);
            date = date.Add(TimeSpan.FromSeconds(1));
            double force = SharedVariables.LoadcellScaled;
            double enc = SharedVariables.EncoderScaled;
            int ana = SharedVariables.AnaTest;

            DynamicData.Add(new Data(date, force, enc, ana));
            
                
         }
        
        public ObservableCollection<Data> GenerateData()
        {
            ObservableCollection<Data> datas = new ObservableCollection<Data>();

            DateTime date = new DateTime(2009, 1, 1);
            double force = SharedVariables.LoadcellScaled;
            double enc = SharedVariables.EncoderScaled;
            int ana = SharedVariables.AnaTest;
           
            for (int i = 0; i < this.DataCount; i++)
            {
                datas.Add(new Data(date, force, enc, ana));
                date = date.Add(TimeSpan.FromSeconds(1));
                
                    force = SharedVariables.LoadcellScaled;
                    enc = SharedVariables.EncoderScaled;
                    ana = SharedVariables.AnaTest;

            }
            
            return datas;
        }
        
        private void timer_Tick(object sender, EventArgs e)
        {
            AddData();
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
        public SharerConnection connection;
        public bool ManSelect;
        public bool ManUP;
        public bool ManDown;
        public bool Autostart;
        private int isWorking;
        private System.Threading.Timer ReadArduinoTmr;

        DispatcherTimer UpdateUItmr;

        public MainWindow()
        {
            InitializeComponent();
            
            sldManSpd.Value = 0;
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
            
            //StoreValues();
        }

        private void UpdateUItmr_Tick(object sender, EventArgs e)
        {

            txt_Force.Value = SharedVariables.AnaTest;
            SharedVariables.EncoderScaled = ((SharedVariables.EncoderRaw / 300.25));
            txt_Dist.Value = SharedVariables.EncoderScaled;
            //txt_Ana.Value = SharedVariables.AnaTest;
            txt_SR.Text = SharedVariables.Servo_Rdy.ToString();
            txt_SZ.Text = SharedVariables.Servo_ZSpd.ToString();
        }
        

        private void MenuItem_Close_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            Application.Current.Shutdown();
            Close();
        }
       
        private void txt_CalFactor_TextChanged(object sender, TextChangedEventArgs e)
        {
            //connection.WriteVariable("calibration_factor", txt_CalFactor.Value);
        }
        #region Buttons

        private void btn_Up_Click(object sender, RoutedEventArgs e)
        {

            if (ManUP == false && ManSelect == true && ManDown == false)
            {
                SlideUP();
                btn_Up.Content = "Stop";
                ManUP = true;
            }
            else
            {
                SlideUpStop();
                btn_Up.Content = "Up";
                ManUP = false;
            }

        }

        private void btn_Down_Click(object sender, RoutedEventArgs e)
        {
            if (ManDown == false && ManSelect == true && ManUP == false)
            {
                SlideDown();
                btn_Down.Content = "Stop";
                ManDown = true;
            }
            else
            {
                SlideDownStop();
                btn_Down.Content = "Down";
                ManDown = false;
            }

        }

        private void btn_Man_Click(object sender, RoutedEventArgs e)
        {
            if (ManSelect == false)
            {
                btn_Man.Content = "Manual";
                ManSelect = true;

            }
            else
            {
                btn_Man.Content = "Auto";
                ManSelect = false;
                SlideUpStop();
                ManUP = false;
                btn_Up.Content = "Up";
                SlideDownStop();
                btn_Down.Content = "Down";
                ManDown = false;
            }

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

        private void btnIB_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnMOE_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            
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

        private void btnAutoStart_Click(object sender, RoutedEventArgs e)
        {
            if (Autostart == false)
            {
                btnAutoStart.Content = "Stop";
                btnAutoStart.Background = new SolidColorBrush(Colors.Red);
                Autostart = true;

            }
            else
            {
                btnAutoStart.Content = "Start";
                btnAutoStart.Background = new SolidColorBrush(Colors.Green);
                Autostart = false;

            }
            #endregion
        }
        private void WriteSpeedRef()
        {
            int SpeedRef;

            if (ManSelect == true)
            {
                SpeedRef = ManSpeed * 25;
            }
            else
            {
                SpeedRef = AutoSpeedTest * 25;
            }

                try
                {
                   
                    connection.WriteVariable("SpeedRef", SpeedRef * 25);

                }
                catch (Exception)
                {
                    MessageBox.Show("Connection Lost!! Resolve issue and reconnect");

                }
            
        }

        private void SlideUP()
        {
            var Up = connection.Call("SlideUp");
        }

        private void SlideUpStop()
        {
            var Up = connection.Call("SlideUpStop");
        }

        private void SlideDown()
        {
            var Down = connection.Call("SlideDown");
        }

        private void SlideDownStop()
        {
            var Down = connection.Call("SlideDownStop");
        }

    }
}
