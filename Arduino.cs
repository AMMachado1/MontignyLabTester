using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharer;
public SharerConnection connection;

namespace TensileTesterSharer
{
    public class Arduino
    {
        var values = connection.ReadVariables(new string[] { "encoder", "Loadcell_Mass", "ana", "Srdy", "Szpd" });

                SharedVariables.EncoderRaw = Convert.ToDouble(values[0].Value);
                SharedVariables.LoadcellScaled = Convert.ToDouble(values[1].Value);
                SharedVariables.AnaTest = Convert.ToInt16(values[2].Value);
                SharedVariables.Servo_Rdy = Convert.ToBoolean(values[3].Value);
                SharedVariables.Servo_ZSpd = Convert.ToBoolean(values[4].Value);
                
            
    }
}
