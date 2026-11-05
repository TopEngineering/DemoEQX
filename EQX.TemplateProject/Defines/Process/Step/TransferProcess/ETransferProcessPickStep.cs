using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.TemplateProject.Defines.Process.Step.TransferProcess
{
    public enum ETransferProcessPickStep
    {
        Start,
        XAxis_MovePickPosition,
        XAxis_MovePickPosition_Wait,

        YAxis_MovePickPosition,
        YAxis_MovePickPosition_Wait,

        ZAxis_MovePickPosition,
        ZAxis_MovePickPosition_Wait,

        Cylinders_Down,
        Cylinders_Down_Wait,

        Cylinders_Up,
        Cylinders_Up_Wait,

        End,
    }
}
