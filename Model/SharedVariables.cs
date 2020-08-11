using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static bool MOETestStartred;
        public static bool IBTestStartred;
        public static bool TestComplete;
    }
}
