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

        TrayPlace1Request,
        TrayPlace1Request_Wait,
        Cylinders1_Down,
        Cylinders1_Down_Wait,
        Vacuum1_Blow,
        Vacuum1_Blow_Wait,
        Cylinders1_Up,
        Cylinders1_Up_Wait,
        TrayPlace1Done,
        TrayPlace1Done_Wait,

        ZAxis_MovePlaceHead2Position,
        ZAxis_MovePlaceHead2Position_Wait,

        TrayPlace2Request,
        TrayPlace2Request_Wait,
        Cylinders2_Down,
        Cylinders2_Down_Wait,
        Vacuum2_Blow,
        Vacuum2_Blow_Wait,
        Cylinders2_Up,
        Cylinders2_Up_Wait,
        TrayPlace2Done,
        TrayPlace2Done_Wait,

        End,
    }
}
