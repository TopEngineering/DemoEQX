namespace EQX.TemplateProject.Defines.Process.Step.TransferProcess
{
    public enum ETransferProcessAutoRunStep
    {
        Start,
        ZAxis_MoveTravelPosition,
        ZAxis_MoveTravelPosition_Wait,
        WaitInputTray,
        WaitInputTray_Wait,
        WaitOutputTray,
        WaitOutputTray_Wait,
        End,
    }
}
