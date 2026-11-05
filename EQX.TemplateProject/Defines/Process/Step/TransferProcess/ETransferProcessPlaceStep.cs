using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.TemplateProject.Defines.Process.Step.TransferProcess
{
    public enum ETransferProcessPlaceStep
    {
        Start,
        XAxis_MovePlacePosition,
        XAxis_MovePlacePosition_Wait,

        YAxis_MovePlacePosition,
        YAxis_MovePlacePosition_Wait,

        ZAxis_MovePlacePosition,
        ZAxis_MovePlacePosition_Wait,

        Cylinders_Down,
        Cylinders_Down_Wait,

        Cylinders_Up,
        Cylinders_Up_Wait,

        End,
    }
}
