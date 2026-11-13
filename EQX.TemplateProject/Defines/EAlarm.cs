namespace EQX.TemplateProject.Defines
{
    public enum EAlarm
    {
        None = -1,

        // System Level Alarms
        MainAirNotSupplied = 0,
        MainPowerDown = 1,
        MotionAlarmDetected = 2,
        DoorOpen = 3,
        LightCurtainLeftDetected = 4,
        LightCurtainRightDetected = 5,
        EmergencyStopActivated = 6,
        PowerMCOff = 7,
        Motion_Driver_Off,
        Motion_Limit_Detected,
        Motion_Alarm_Detected,

        TransferZAxis_OriginTimeout = 100,
        TransferXAxis_OriginTimeout,
        TransferYAxis_OriginTimeout,

        TransferXAxis_PickTimeout,
        TransferYAxis_PickTimeout,
        TransferZAxis_PickTimeout,

        TransferXAxis_PlaceTimeout,
        TransferYAxis_PlaceTimeout,
        TransferZAxis_PlaceTimeout,

        TransferCylinder_Pick1Timeout,
        TransferCylinder_Pick2Timeout,
        TransferCylinder_Place1Timeout,
        TransferCylinder_Place2Timeout,

        Vacuum1_OnTimeout,
        Vacuum2_OnTimeout,
        Vacuum1_BlowTimeout,
        Vacuum2_BlowTimeout,

        TransferCylinder1_ReadyTimeout,
        TransferCylinder2_ReadyTimeout,
        TransferZAxis_ReadyTimeout,
        TransferInputTrayTimeout,
        TransferOutputTrayTimeout,
        TransferXAxis_ReadyTimeout,
        TransferYAxis_ReadyTimeout,
        TransferNotOrigin,

        TrayRecipeInvalid = 200,
        TrayInputEmpty,
        TrayOutputFull,
        TrayPickTimeout,
        TrayPlaceTimeout,
        TransferRequestPickFailed,
        TransferRequestPlaceFailed,
        TransferPickFailed,
        TransferPlaceFailed,
    }
}
