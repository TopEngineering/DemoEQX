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

        Cylinders1_Down,
        Cylinders1_Down_Wait,
        Vacuum1_On,
        Vacuum1_On_Wait,
        Cylinders1_Up,
        Cylinders1_Up_Wait,

        Cylinders2_Down,
        Cylinders2_Down_Wait,
        Vacuum2_On,
        Vacuum2_On_Wait,
        Cylinders2_Up,
        Cylinders2_Up_Wait,
        End,
    }
}
