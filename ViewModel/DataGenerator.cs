using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Threading;

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
            if (SharedVariables.ResetChart == true)
            {
                DateTime date = new DateTime(2009, 1, 1);
                double force = 0;
                double enc = 0;
                double ana = 0;

                DynamicData.Add(new Data(date, force, enc, ana));
                SharedVariables.ResetChart = false;
            }
            else
            {
                DateTime date = new DateTime(2009, 1, 1);
                date = date.Add(TimeSpan.FromSeconds(1));
                double force = SharedVariables.LoadcellScaled;
                double enc = SharedVariables.EncoderScaled;
                double ana = SharedVariables.AnaTest;

                DynamicData.Add(new Data(date, force, enc, ana));
            }

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
            if (SharedVariables.MOETestStarted == true || SharedVariables.IBTestStarted == true)
            {
                SharedVariables.ForceSample2 = SharedVariables.ForceSample1;
                SharedVariables.ForceSample1 = SharedVariables.AnaTest;
                if ((SharedVariables.ForceSample2 - SharedVariables.ForceSample1) > SharedVariables.BreakForce)
                {
                    SharedVariables.TestComplete = true;

                }
                if (SharedVariables.AnaTest > SharedVariables.MaxForce)
                {
                    SharedVariables.MaxForce = SharedVariables.AnaTest;
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


