using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.TemplateProject.Defines.Process.Step.TransferProcess
{
    public enum ETransferProcessReadyStep
    {
        Start,
        CheckOriginDone,
        Cylinders_Up,
        Cylinders_Up_Wait,
        XAxis_Move_Ready,
        XAxis_Move_Ready_Wait,
        YAxis_Move_Ready,
        YAxis_Move_Ready_Wait,
        ZAxis_Move_Ready,
        ZAxis_Move_Ready_Wait,
        End,
    }
}
