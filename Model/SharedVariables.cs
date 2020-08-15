using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TensileTesterSharer
{
    public class SharedVariables
    {
        public static double EncoderScaled;
        public static double LoadcellScaled;
        public static double MaxForce;
        public static double ForceSample1;
        public static double ForceSample2;
        public static double BreakForce;
        public static double EncoderRaw;
        public static double LoadcellRaw;
        public static double ReturnOffset;
        public static double AnaTest;
        public static double LoadTest;
        public static bool Servo_Rdy;
        public static bool Servo_ZSpd;
        public static bool MOETestStarted;
        public static bool IBTestStarted;
        public static bool TestComplete;
        public static double EncFac;
        public static float LoadFac;
        public static bool ResetChart;
        public static Image img1T;
        public static Image img2B;
        public static Image img3T;
        public static Image img4B;
        public static Image img5T;
        public static Image img6B;
        public static Image img7T;
        public static Image img8B;
        public static double Length;
        public static double Width;
        public static double Thickness;
        public static double Density;
    }
}
