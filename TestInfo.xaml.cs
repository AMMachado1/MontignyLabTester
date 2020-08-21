using Syncfusion.Windows.Shared;
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

namespace TensileTesterSharer
{
    /// <summary>
    /// Interaction logic for TestInfo.xaml
    /// </summary>
    public partial class TestInfo : Window
    {
        public TestInfo()
        {
            InitializeComponent();
            cmbShift.Text = "";
            txtForeman.Text = "";
            txtOperator.Text = "";
            txtLength.Value = 0.00;
            txtWidth.Value = 0.00;
            txtThickness.Value = 0.00;
            txtDensity.Value = 0.00;
            
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((string.IsNullOrWhiteSpace(cmbShift.Text)) || (string.IsNullOrWhiteSpace(txtForeman.Text)) || (string.IsNullOrWhiteSpace(txtOperator.Text)) || (txtLength.Value == 0.00)
                    || (txtWidth.Value == 0.00) || (txtThickness.Value == 0.00))
                {
                    MessageBox.Show("Please Enter all the Required Details");
                }
                else
                {
                   
                    SharedVariables.Shift = cmbShift.SelectedItem.ToString();
                    SharedVariables.Foreman = txtForeman.Text;
                    SharedVariables.Operator = txtOperator.Text;
                    SharedVariables.Length = (double)txtLength.Value;
                    SharedVariables.Width = (double)txtWidth.Value;
                    SharedVariables.Thickness = (double)txtThickness.Value;
                    SharedVariables.Density = (double)txtDensity.Value;
                    this.DialogResult = true;
                }

            }
            catch (Exception st)
            {
                MessageBox.Show(st.ToString());
               
            }
           
           
           
        }
    }
}
