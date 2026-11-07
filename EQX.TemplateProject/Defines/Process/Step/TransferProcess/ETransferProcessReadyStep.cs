namespace EQX.TemplateProject.Defines.Process.Step.TransferProcess
{
    public enum ETransferProcessReadyStep
    {
        Start,
        CheckOriginDone,
        Cylinder1_Up,
        Cylinder1_Up_Wait,
        Cylinder2_Up,
        Cylinder2_Up_Wait,
        XAxis_MoveReadyPosition,
        XAxis_MoveReadyPosition_Wait,
        YAxis_MoveReadyPosition,
        YAxis_MoveReadyPosition_Wait,
        ZAxis_MoveReadyPosition,
        ZAxis_MoveReadyPosition_Wait,
        End,
    }
}
