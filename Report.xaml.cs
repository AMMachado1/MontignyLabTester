using Syncfusion.Windows.Tools.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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
using SUT.PrintEngine.Utils;



namespace TensileTesterSharer
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : Window
    {
        public Report()
        {
            InitializeComponent();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            /*
            var visualSize = new Size(Print.ActualWidth, Print.ActualHeight);
            var printControl = PrintControlFactory.Create(visualSize, Print);
            printControl.ShowPrintPreview();
            */
            try
            {
                PrintDialog pd = new PrintDialog();
                Window window = Application.Current.MainWindow;
                Application.Current.MainWindow = this;
                if ((bool)pd.ShowDialog().GetValueOrDefault())
                {
                    Application.Current.MainWindow = window;
                    pd.PrintVisual(this, "Test Report");
                }
            }
            catch (Exception p)
            {

                MessageBox.Show("Print Error" + p);
            }
            
        }
    }
}
