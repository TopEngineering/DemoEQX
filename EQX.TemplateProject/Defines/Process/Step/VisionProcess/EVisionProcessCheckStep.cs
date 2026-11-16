namespace EQX.TemplateProject.Defines.Process.Step.VisionProcess
{
    public enum EVisionProcessCheckStep
    {
        Start,
        SetCurrentHead,
        Cylinder_Move_VisionPosition,
        Cylinder_Move_VisionPosition_Wait,

        ImageGrab,
        ImageGrab_Wait,

        VisionInspect,
        VisionInspect_Wait,

        ResultUpdate,
        UpdateCurrentHead,

        Vision_CheckDone,
        Vision_ClearFlags,
        End,
    }
}
