using System;
using System.Collections.Generic;
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
using TensileTesterSharer.Properties;

namespace TensileTesterSharer
{
    /// <summary>
    /// Interaction logic for Calibration.xaml
    /// </summary>
    public partial class Calibration : Window
    {
        public Calibration()
        {
            InitializeComponent();
           
            txtEncFac.Value = Settings.Default.EncFac;
                      
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            SharedVariables.EncFac = (float)txtEncFac.Value;
            Settings.Default.EncFac = SharedVariables.EncFac;
            Settings.Default.Save();
            SharedVariables.LoadFac = (UInt16)txtLoadFac.Value;
           
            this.DialogResult = true;
        }

        private void btnTare_Click(object sender, RoutedEventArgs e)
        {
            SharedVariables.CalWinTare = true;
        }

        private void btnCal_Click(object sender, RoutedEventArgs e)
        {
            SharedVariables.CalWinCal = true;
            SharedVariables.LoadFac = (ushort)txtLoadFac.Value;
        }
    }
}
