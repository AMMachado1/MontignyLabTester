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
            txtDate.Text = System.DateTime.Now.ToString("yy/MM/dd");
            txtTime.Text = DateTime.Now.ToString("hh:mm:ss");
            txtShf1.Value = SharedVariables.Shf1;
            txtShf2.Value = SharedVariables.Shf2;
            txtShf3.Value = SharedVariables.Shf3;
            txtShf4.Value = SharedVariables.Shf4;
            txtShfAvg.Value = SharedVariables.ShfAvg;
            txtShe1.Value = SharedVariables.She1;
            txtShe2.Value = SharedVariables.She2;
            txtShe3.Value = SharedVariables.She3;
            txtShe4.Value = SharedVariables.She4;
            txtSheAvg.Value = SharedVariables.SheAvg;
            txtFst1.Value = SharedVariables.Fst1;
            txtFst2.Value = SharedVariables.Fst2;
            txtFst3.Value = SharedVariables.Fst3;
            txtFst4.Value = SharedVariables.Fst4;
            txtFstAvg.Value = SharedVariables.FstAvg;
            txtFsb1.Value = SharedVariables.Fsb1;
            txtFsb2.Value = SharedVariables.Fsb2;
            txtFsb3.Value = SharedVariables.Fsb3;
            txtFsb4.Value = SharedVariables.Fsb4;
            txtFsbAvg.Value = SharedVariables.FsbAvg;
            txt1Thick.Value = SharedVariables.IB1Thick;
            txt2Thick.Value = SharedVariables.IB2Thick;
            txt3Thick.Value = SharedVariables.IB3Thick;
            txt4Thick.Value = SharedVariables.IB4Thick;
            txt5Thick.Value = SharedVariables.IB5Thick;
            txt6Thick.Value = SharedVariables.IB6Thick;
            txt7Thick.Value = SharedVariables.IB7Thick;
            txt8Thick.Value = SharedVariables.IB8Thick;
            txtAvgThick.Value = SharedVariables.IBAvgThick;
            txt1Read.Value = SharedVariables.IB1Read;
            txt2Read.Value = SharedVariables.IB2Read;
            txt3Read.Value = SharedVariables.IB3Read;
            txt4Read.Value = SharedVariables.IB4Read;
            txt5Read.Value = SharedVariables.IB5Read;
            txt6Read.Value = SharedVariables.IB6Read;
            txt7Read.Value = SharedVariables.IB7Read;
            txt8Read.Value = SharedVariables.IB8Read;
            txtAvgRead.Value = SharedVariables.IBAvgRead;
            txt1Mpa.Value = SharedVariables.IB1Mpa;
            txt2Mpa.Value = SharedVariables.IB2Mpa;
            txt3Mpa.Value = SharedVariables.IB3Mpa;
            txt4Mpa.Value = SharedVariables.IB4Mpa;
            txt5Mpa.Value = SharedVariables.IB5Mpa;
            txt6Mpa.Value = SharedVariables.IB6Mpa;
            txt7Mpa.Value = SharedVariables.IB7Mpa;
            txt8Mpa.Value = SharedVariables.IB8Mpa;
            txtAvgMpa.Value = SharedVariables.IBAvgMpa;
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

        private void btnTest1_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
