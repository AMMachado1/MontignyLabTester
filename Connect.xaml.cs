using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TensileTesterSharer
{
    /// <summary>
    /// Interaction logic for Connect.xaml
    /// </summary>
    public partial class Connect : Window
    {
        public Connect()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            foreach (string comport in ports)
            {
                cmb_Port.Items.Add(comport);
            }

        }

        private void btn_connect_Click(object sender, RoutedEventArgs e)
        {
            SharedVariables.ComPort = cmb_Port.SelectedItem.ToString();
            this.DialogResult = true;
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string comport in ports)
            {
                cmb_Port.Items.Add(comport);
            }
        }
    }
}
