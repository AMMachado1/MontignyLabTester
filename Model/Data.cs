using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TensileTesterSharer
{
    public class Data
    {
        public Data(double force, double enc, double ana)
        {
            Force = force;
            Enc = enc;
            Ana = ana;
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

        public double Ana
        {
            get;
            set;
        }

    }
}
