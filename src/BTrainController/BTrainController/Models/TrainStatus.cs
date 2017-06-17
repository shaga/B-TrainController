using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTrainController.Models
{
    public enum ETrainStatus
    {
        Initializing,
        Stop,
        CanGo,
        BeforeGo,
        Going,
        GoingMax,
        BeforeStop,
        Stopping,
    }
}
