using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTrainController.Models
{
    public enum ESpeedLevel
    {
        Stop = BleTrain.SpeedStop,
        Lv1 = BleTrain.SpeedMin,
        Lv2 = BleTrain.SpeedMin + 0x10,
        Lv3 = BleTrain.SpeedMin + 0x20,
        Lv4 = BleTrain.SpeedMin + 0x30,
    }
}
