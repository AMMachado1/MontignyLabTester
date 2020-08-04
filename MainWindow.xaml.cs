using Sharer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TensileTesterSharer
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double EncoderRaw = 0;
        public float EncoderScaled = 0;
        public float LoadcellRaw = 0;
        public float LoadcellScaled = 0;
        public int ManSpeed = 0;
        public int AutoSpeedTest = 0;
        public int AutoSpeedApproach = 0;
        
        public bool ManSelect;
        public bool ManUP;
        public bool ManDown;
        public SharerConnection connection;
        private int isWorking;
        private System.Threading.Timer UITimer;
        //public connection;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModel();
            txt_SpeedRef.Value = 0;
            string[] ports = SerialPort.GetPortNames();
            foreach (string comport in ports)
            {
                cmb_Port.Items.Add(comport);
            }
            connect();
            
        }

        public void connect()
        {
            connection = new SharerConnection("COM23", 115200);
            //connection = new SharerConnection(cmb_Port.Text, 115200);
            connection.Connect();
            connection.RefreshFunctions();

            if (connection.Connected)
            {
                UITimer = new System.Threading.Timer(UpdateUI, null, 500, 500);
                var CalFactorTemp = connection.ReadVariable("calibration_factor");
                txt_CalFactor.Value = Convert.ToInt32(CalFactorTemp.Value);

            }
            else
            {
                MessageBox.Show("Cannot Connect, Please check Cabling and Retry");
            }

        }

        #region Charts
        
        public class Model
        {
            public TimeSpan XValue { get; set; }
            public double YValue { get; set; }
        }

        public class ViewModel
        {
            public int Index { get; set; }

            public ViewModel()
            {
                Data = new ObservableCollection<Model>();
                Random rand = new Random();

                var random = new Random();
                for (int i = 0; i < 100; i++)
                {
                    Data.Add(new Model() { XValue = TimeSpan.FromSeconds(Index), YValue = rand.Next(50, 100) });
                    Index++;
                }
            }

            public ObservableCollection<Model> Data { get; set; }
        }
        #endregion

        private void btn_Up_Click(object sender, RoutedEventArgs e)
        {
           
            if (ManUP == false && ManSelect == true && ManDown == false)
            {
                var Up = connection.Call("SlideUp");
                btn_Up.Content = "Stop";
                ManUP = true;
            }
            else
            {
                var Up = connection.Call("SlideUpStop");
                btn_Up.Content = "Up";
                ManUP = false;
            }

        }

        private void btn_Down_Click(object sender, RoutedEventArgs e)
        {
            if (ManDown == false && ManSelect == true && ManUP == false)
            {
                var Up = connection.Call("SlideDown");
                btn_Down.Content = "Stop";
                ManDown = true;
            }
            else
            {
                var Up = connection.Call("SlideDownStop");
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
                var Up = connection.Call("SlideUpStop");
                ManUP = false;
                btn_Up.Content = "Up";
                var Down = connection.Call("SlideDownStop");
                btn_Down.Content = "Down";
                ManDown = false;
            }

        }

        private void UpdateUI(object state)
        {
            if (connection.Connected)
            {

                if ((Interlocked.Increment(ref isWorking) == 1))
                {
                    _ = this.Dispatcher.BeginInvoke((System.Action)(() =>
                    {

                        try
                        {
                        //txt_Force.Text = connection.Call("analogRead", 0).ToString();
                            var values = connection.ReadVariables(new string[] { "counter", "Loadcell_Mass" });
                            EncoderRaw = Convert.ToInt32(values[0].Value);
                            EncoderScaled = ((float)(EncoderRaw / 300.0));
                            txt_Dist.Text = EncoderScaled.ToString();
                            gaugeDist.Value = EncoderScaled.ToString();
                            txt_Force.Text = Convert.ToInt32(values[1].Value).ToString();
                            
                        }

                        catch (NullReferenceException)
                        {
                            MessageBox.Show("Null Ref");
                        }

                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
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
                UITimer.Dispose();
                MessageBox.Show("Connection Lost!! Resolve issue and reconnect");
            }
        }

        private void MenuItem_Close_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            Application.Current.Shutdown();
            Close();
        }

        private void txt_SpeedRef_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ManSpeed = (int)txt_SpeedRef.Value;
                connection.WriteVariable("SpeedRef", ManSpeed);

            }
            catch (Exception)
            {
                MessageBox.Show("Connection Lost!! Resolve issue and reconnect");

            }
            
            
        }

        private void txt_CalFactor_TextChanged(object sender, TextChangedEventArgs e)
        {
            //connection.WriteVariable("calibration_factor", txt_CalFactor.Value);
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
    }
}
