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

        TrayPick1Request,
        TrayPick1Request_Wait,
        Cylinders1_Down,
        Cylinders1_Down_Wait,
        Vacuum1_On,
        Vacuum1_On_Wait,
        Cylinders1_Up,
        Cylinders1_Up_Wait,
        TrayPick1Done,
        TrayPick1Done_Wait,
        TrayPick1DoneClear_Wait,

        ZAxis_MovePickHead2Position,
        ZAxis_MovePickHead2Position_Wait,

        TrayPick2Request,
        TrayPick2Request_Wait,
        Cylinders2_Down,
        Cylinders2_Down_Wait,
        Vacuum2_On,
        Vacuum2_On_Wait,
        Cylinders2_Up,
        Cylinders2_Up_Wait,
        TrayPick2Done,
        TrayPick2Done_Wait,
        TrayPick2DoneClear_Wait,

        XAxis_MoveInspectVision1,
        XAxis_MoveInspectVision1_Wait,
        YAxis_MoveInspectVision1,
        YAxis_MoveInspectVision1_Wait,
        ZAxis_MoveInspectVision1,
        ZAxis_MoveInspectVision1_Wait,
        XAxis_MoveInspectVision2,
        XAxis_MoveInspectVision2_Wait,
        YAxis_MoveInspectVision2,
        YAxis_MoveInspectVision2_Wait,
        ZAxis_MoveInspectVision2,
        ZAxis_MoveInspectVision2_Wait,
        IfSkipVision,
        End,
    }
}
