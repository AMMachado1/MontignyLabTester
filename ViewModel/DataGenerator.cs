using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace TensileTesterSharer.ViewModel
{/*
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
            timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
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
            SharedVariables.EncoderScaled = ((SharedVariables.EncoderRaw / 300.25));
            //txt_Dist.Value = SharedVariables.EncoderScaled;
            AddData();
            //GenerateData();
        }

    }
    */
}

