namespace EQX.TemplateProject.Defines.Process.Step.TrayProcess
{
    public enum ETrayProcessPreparePickCellStep
    {
        Start,
        CheckInputTray,
        TransferRequestPick,
        TrayPickCellReady,
        TransferPickDone,
        TrayPickDone,
        UpdateInputIndex,
        ClearPickFlags,
        End,
    }
}
