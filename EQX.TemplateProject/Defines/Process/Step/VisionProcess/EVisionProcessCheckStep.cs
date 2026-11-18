namespace EQX.TemplateProject.Defines.Process.Step.VisionProcess
{
    public enum EVisionProcessCheckStep
    {
        Start,
        IfSkipVision,
        SetCurrentHead,
        Cylinder_Move_VisionPosition,
        Cylinder_Move_VisionPosition_Wait,

        ImageGrab,
        ImageGrab_Wait,

        VisionInspect,
        VisionInspect_Wait,

        VisionRetryCheck,
        ResultUpdate,
        UpdateCurrentHead,

        Vision_CheckDone,
        RequestTransferPlace,
        TransferRequestOff_Wait,
        Vision_ClearFlags,
        End,
    }
}
