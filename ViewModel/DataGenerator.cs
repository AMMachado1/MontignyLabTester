using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Syncfusion.UI.Xaml.Charts;
using System.Windows.Data;

namespace TensileTesterSharer
{
    public class DataGenerator
    {
        private ObservableCollection<Data> Data;
        DispatcherTimer timer;

        public ObservableCollection<Data> DynamicData { get; set; }

        public DataGenerator()
        {

            DynamicData = new ObservableCollection<Data>();
            Data = new ObservableCollection<Data>();
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
            if (SharedVariables.ResetChart == true)
            {
                double force = 0;
                double enc = 0;
                double ana = 0;

                DynamicData.Add(new Data(force, enc, ana));
                SharedVariables.ResetChart = false;
               
            }
            else
            {
                double force = SharedVariables.LoadcellScaled;
                double enc = SharedVariables.EncoderScaled;
                double ana = SharedVariables.AnaTest;

                DynamicData.Add(new Data(force, enc, ana));
            }
           
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (SharedVariables.MOETestStarted == true || SharedVariables.IBTestStarted == true)
            {
                SharedVariables.ForceSample2 = SharedVariables.ForceSample1;
                SharedVariables.ForceSample1 = SharedVariables.AnaTest;
                if ((SharedVariables.ForceSample2 - SharedVariables.ForceSample1) > SharedVariables.BreakForce)
                {
                    SharedVariables.TestComplete = true;

                }
                if (SharedVariables.LoadcellScaled > SharedVariables.MaxForce)
                {
                    SharedVariables.MaxForce = SharedVariables.LoadcellScaled;
                }
                AddData();
            }
            else if (SharedVariables.ResetChart == true)
            {
                AddData();
            }

        }

    }

}


