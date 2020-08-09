using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TensileTesterSharer
{
    public class Data
    {
        public Data(DateTime date, double force, double enc, int ana)
        {
            Date = date;
            Force = force;
            Enc = enc;
            Ana = ana;
           

        }

        public DateTime Date
        {
            get;
            set;
        }

        public double Force
        {
            get;
            set;
        }
        public double Enc
        {
            get;
            set;
        }

        public int Ana
        {
            get;
            set;
        }
        
    }
}
