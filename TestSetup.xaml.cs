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


namespace TensileTesterSharer.Properties
{
    /// <summary>
    /// Interaction logic for TestSetup.xaml
    /// </summary>
    public partial class TestSetup : Window
    {
        public TestSetup()
        {
            InitializeComponent();
            txtReturnOffset.Value = Settings.Default.RetOffset;
            txtTestBrkForce.Value = Settings.Default.BrkForce;
            txtEncFac.Value = Settings.Default.EncFac;
            txtLoadFac.Value = Settings.Default.LoadFac;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            SharedVariables.ReturnOffset = (double)txtReturnOffset.Value;
            Settings.Default.RetOffset = SharedVariables.ReturnOffset;
            SharedVariables.BreakForce = (double)txtTestBrkForce.Value;
            Settings.Default.BrkForce = SharedVariables.BreakForce;
            SharedVariables.EncFac = (double)txtEncFac.Value;
            Settings.Default.EncFac = SharedVariables.EncFac;
            SharedVariables.LoadFac = (float)txtLoadFac.Value;
            Settings.Default.LoadFac = SharedVariables.LoadFac;
            Settings.Default.Save();
            this.DialogResult = true;
        }
    }
}
