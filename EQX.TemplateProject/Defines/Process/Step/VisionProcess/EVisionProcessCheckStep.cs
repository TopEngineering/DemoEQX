namespace EQX.TemplateProject.Defines.Process.Step.VisionProcess
{
    public enum EVisionProcessCheckStep
    {
        Start,
        FlagTranserREQInspect,
        SetCurrentHead,
        Head_Move_UnderVisionPosition_Wait,
        ImageGrab,
        ImageGrab_Wait,
        VisionInspect1,
        VisionInspect1_Wait,
        VisionInspect2,
        VisionInspect2_Wait,
        VisionRetryCheck,
        ResultUpdate,
        UpdateCurrentHead,
        Vision_CheckDone,
        Vision_ClearFlags,
        End,
    }
}
