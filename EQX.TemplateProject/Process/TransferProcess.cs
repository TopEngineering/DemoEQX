using EQX.TemplateProject.MVVM.Models;
using EQX.Core.InOut;
using EQX.Core.Motion;
using EQX.Core.Sequence;
using EQX.Process;
using EQX.TemplateProject.Defines;
using EQX.TemplateProject.Defines.Devices;
using EQX.TemplateProject.Defines.Process.Step.TransferProcess;
using EQX.TemplateProject.Recipe;

namespace EQX.TemplateProject.Process
{
    public class TransferProcess : ProcessBase<ESequence>
    {
        #region Privates
        private readonly Devices _devices;
        private readonly VirtualIO _virtualIO;
        private readonly RecipeList _recipeList;

        private IMotion XAxis => _devices.Motions.TransferXAxis;
        private IMotion YAxis => _devices.Motions.TransferYAxis;
        private IMotion ZAxis => _devices.Motions.TransferZAxis;
        private ICylinder Cylinder1 => _devices.Cylinders.TransferCylinder1;
        private ICylinder Cylinder2 => _devices.Cylinders.TransferCylinder2;
        #endregion

        #region Flags
        private IDInputDevice VirtualInput => _virtualIO.TransferProcessInput;
        private IDOutputDevice VirtualOutput => _virtualIO.TransferProcessOutput;
        private bool FlagRequestVisionInspect1
        {
            set { VirtualOutput[(int)ETransferProcessOutput.TRANSFER_REQ_INSPECT1] = value; }
        }
        private bool FlagRequestVisionInspect2
        {
            set { VirtualOutput[(int)ETransferProcessOutput.TRANSFER_REQ_INSPECT2] = value; }
        }
        private bool FlagVisionInspect1Done
        {
            get { return VirtualInput[(int)ETransferProcessInput.VISION_INSPECT1_DONE]; }
        }
        private bool FlagVisionInspect2Done
        {
            get { return VirtualInput[(int)ETransferProcessInput.VISION_INSPECT2_DONE]; }
        }
        #endregion

        #region Constructor
        public TransferProcess(Devices devices,
            VirtualIO virtualIO,
            RecipeList recipeList,
            MachineStatus machineStatus)
        {
            _devices = devices;
            _virtualIO = virtualIO;
            _recipeList = recipeList;
        }
        #endregion

        #region Override Methods
        public override bool ProcessOrigin()
        {
            switch ((ETransferProcessOriginStep)Step.OriginStep)
            {
                case ETransferProcessOriginStep.Start:
                    Log.Debug("Origin Start");
                    Step.OriginStep++;
                    break;
                case ETransferProcessOriginStep.XAxis_Origin:
                    Log.Debug("X Axis Origin");
                    XAxis.SearchOrigin();
                    Wait((int)(_recipeList.CommonRecipe.MotionOriginTimeout * 1000), () => XAxis.Status.IsHomeDone);
                    Step.OriginStep++;
                    break;
                case ETransferProcessOriginStep.XAxis_Origin_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferXAxis_OriginTimeout);
                        return false;
                    }
                    Log.Debug("X Axis Origin Done");
                    Step.OriginStep++;
                    break;
                case ETransferProcessOriginStep.YAxis_Origin:
                    Log.Debug("Y Axis Origin");
                    YAxis.SearchOrigin();
                    Wait((int)(_recipeList.CommonRecipe.MotionOriginTimeout * 1000), () => YAxis.Status.IsHomeDone);
                    Step.OriginStep++;
                    break;
                case ETransferProcessOriginStep.YAxis_Origin_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferYAxis_OriginTimeout);
                        return false;
                    }
                    Log.Debug("Y Axis Origin Done");
                    Step.OriginStep++;
                    break;
                case ETransferProcessOriginStep.ZAxis_Origin:
                    Log.Debug("Z Axis Origin");
                    ZAxis.SearchOrigin();
                    Wait((int)(_recipeList.CommonRecipe.MotionOriginTimeout * 1000), () => ZAxis.Status.IsHomeDone);
                    Step.OriginStep++;
                    break;
                case ETransferProcessOriginStep.ZAxis_Origin_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferZAxis_OriginTimeout);
                        return false;
                    }
                    Log.Debug("Z Axis Origin Done");
                    Step.OriginStep++;
                    break;
                case ETransferProcessOriginStep.End:
                    Log.Debug("Origin End");
                    ProcessStatus = EProcessStatus.OriginDone;
                    Step.OriginStep++;
                    break;
                default:
                    Wait(20);
                    break;
            }
            return true;
        }

        public override bool ProcessRun()
        {
            switch (Sequence)
            {
                case ESequence.Stop:
                    Wait(50);
                    break;
                case ESequence.AutoRun:
                    Sequence_AutoRun();
                    break;
                case ESequence.Ready:
                    Sequence_Ready();
                    break;
                case ESequence.Pick:
                    Sequence_Pick();
                    break;
                case ESequence.Inspect:
                    if (Step.RunStep == 0)
                    {
                        if ((!_recipeList.CommonRecipe.SkipCylinder1 && !FlagVisionInspect1Done) ||
                            (!_recipeList.CommonRecipe.SkipCylinder2 && !FlagVisionInspect2Done))
                        {
                            Wait(50);
                            break;
                        }
                        FlagRequestVisionInspect1 = false;
                        FlagRequestVisionInspect2 = false;
                        Log.Debug("Vision completed");
                        Step.RunStep++;
                        break;
                    }
                    if (FlagVisionInspect1Done || FlagVisionInspect2Done)
                    {
                        Wait(50);
                        break;
                    }

                    Log.Debug("Vision Done");
                    Sequence = ESequence.Place;
                    break;
                case ESequence.Place:
                    Sequence_Place();
                    break;
            }
            return true;
        }
        #endregion

        #region Sequences
        private void Sequence_AutoRun()
        {
            switch ((ETransferProcessAutoRunStep)Step.RunStep)
            {
                case ETransferProcessAutoRunStep.Start:
                    Log.Debug("AutoRun Start");
                    Step.RunStep++;
                    break;
                case ETransferProcessAutoRunStep.WaitInputTray:
                    Log.Debug("AutoRun Wait Input Tray");
#if SIMULATION
                    Wait(200);
#else
                    Wait((int)(_recipeList.CommonRecipe.MaterialInputTimeout * 1000),
                        () => _devices.Inputs.InputTrayDetect.Value);
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessAutoRunStep.WaitInputTray_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    { RaiseAlarm(EAlarm.TransferInputTrayTimeout); return; }
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessAutoRunStep.WaitOutputTray:
                    Log.Debug("AutoRun Wait Output Tray");
#if SIMULATION
                    Wait(200);
#else
                    Wait((int)(_recipeList.CommonRecipe.MaterialInputTimeout * 1000),
                        () => _devices.Inputs.OutputTrayDetect.Value);
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessAutoRunStep.WaitOutputTray_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    { RaiseAlarm(EAlarm.TransferOutputTrayTimeout); return; }
                    if (!_devices.Inputs.InputTrayDetect.Value)
                    {
                        Step.RunStep = (int)ETransferProcessAutoRunStep.WaitInputTray;
                        break;
                    }
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessAutoRunStep.End:
                    Log.Debug("AutoRun Ready To Pick");
                    Sequence = ESequence.Pick;
                    break;
            }
        }

        private void Sequence_Ready()
        {
            switch ((ETransferProcessReadyStep)Step.RunStep)
            {
                case ETransferProcessReadyStep.Start:
                    Log.Debug("Ready Start");
                    Step.RunStep++;
                    break;
                case ETransferProcessReadyStep.CheckOriginDone:
                    Log.Debug("Ready Check Origin Done");
                    if (!XAxis.Status.IsHomeDone || !YAxis.Status.IsHomeDone || !ZAxis.Status.IsHomeDone)
                    {
                        RaiseAlarm(EAlarm.TransferNotOrigin);
                        return;
                    }
                    Step.RunStep++;
                    break;
                case ETransferProcessReadyStep.Cylinder1_Up:
                    Log.Debug("Ready Cylinder 1 Up");
                    Cylinder1.Forward();
#if SIMULATION
                    Wait(200);
#else
                    Wait((int)(_recipeList.CommonRecipe.CylinderMoveTimeout * 1000), () => Cylinder1.IsForward);
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessReadyStep.Cylinder1_Up_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    { RaiseAlarm(EAlarm.TransferCylinder1_ReadyTimeout); return; }
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessReadyStep.Cylinder2_Up:
                    Log.Debug("Ready Cylinder 2 Up");
                    Cylinder2.Forward();
#if SIMULATION
                    Wait(200);
#else
                    Wait((int)(_recipeList.CommonRecipe.CylinderMoveTimeout * 1000), () => Cylinder2.IsForward);
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessReadyStep.Cylinder2_Up_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    { RaiseAlarm(EAlarm.TransferCylinder2_ReadyTimeout); return; }
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessReadyStep.XAxis_MoveReadyPosition:
                    Log.Debug("Ready X Axis Move Ready Position");
                    if (!XAxis.MoveAbs(_recipeList.TransferRecipe.XAxisReadyPosition))
                    { RaiseAlarm(EAlarm.TransferXAxis_PickTimeout); return; }
#if SIMULATION
                    Wait(200);
#else
                    Wait((int)(_recipeList.CommonRecipe.MotionMoveTimeOut * 1000),
                        () => XAxis.IsOnPosition(_recipeList.TransferRecipe.ReadyX));
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessReadyStep.XAxis_MoveReadyPosition_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    { RaiseAlarm(EAlarm.TransferXAxis_ReadyTimeout); return; }
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessReadyStep.YAxis_MoveReadyPosition:
                    Log.Debug("Ready Y Axis Move Ready Position");
                    if (!YAxis.MoveAbs(_recipeList.TransferRecipe.YAxisReadyPosition))
                    { RaiseAlarm(EAlarm.TransferYAxis_ReadyTimeout); return; }
#if SIMULATION
                    Wait(200);
#else
                    Wait((int)(_recipeList.CommonRecipe.MotionMoveTimeOut * 1000),
                        () => YAxis.IsOnPosition(_recipeList.TransferRecipe.ReadyY));
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessReadyStep.YAxis_MoveReadyPosition_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    { RaiseAlarm(EAlarm.TransferYAxis_ReadyTimeout); return; }
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessReadyStep.ZAxis_MoveReadyPosition:
                    Log.Debug("Ready Z Axis Move Ready Position");
                    if (!ZAxis.MoveAbs(_recipeList.TransferRecipe.ZAxisReadyPosition))
                    { RaiseAlarm(EAlarm.TransferZAxis_ReadyTimeout); return; }
#if SIMULATION
                    Wait(200);
#else
                    Wait((int)(_recipeList.CommonRecipe.MotionMoveTimeOut * 1000),
                        () => ZAxis.IsOnPosition(_recipeList.TransferRecipe.ReadyZ));
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessReadyStep.ZAxis_MoveReadyPosition_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    { RaiseAlarm(EAlarm.TransferZAxis_ReadyTimeout); return; }
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessReadyStep.End:
                    Log.Debug("Ready End");
                    Sequence = ESequence.Stop;
                    break;
            }
        }

        private void Sequence_Pick()
        {
            switch ((ETransferProcessPickStep)Step.RunStep)
            {
                case ETransferProcessPickStep.Start:
                    Log.Debug("Pick Start");
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.XAxis_MovePickPosition:
                    Log.Debug("X Axis Move to Pick Position");
                    XAxis.MoveAbs(_recipeList.TransferRecipe.XAxisPickPosition);
#if SIMULATION
                    Wait(100);
#else
                    Wait((int)(_recipeList.CommonRecipe.MotionMoveTimeOut * 1000),
                        () => XAxis.IsOnPosition(_recipeList.TransferRecipe.PickX));
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.XAxis_MovePickPosition_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferXAxis_PickTimeout);
                        return;
                    }
#endif
                    Log.Debug("X Axis Move Pick Position Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.YAxis_MovePickPosition:
                    Log.Debug("Y Axis Move to Pick Position");
                    YAxis.MoveAbs(_recipeList.TransferRecipe.YAxisPickPosition);
#if SIMULATION
                    Wait(100);
#else
                    Wait((int)(_recipeList.CommonRecipe.MotionMoveTimeOut * 1000),
                        () => YAxis.IsOnPosition(_recipeList.TransferRecipe.PickY));
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.YAxis_MovePickPosition_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferYAxis_PickTimeout);
                        return;
                    }
#endif
                    Log.Debug("Y Axis Move Pick Position Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.ZAxis_MovePickPosition:
                    Log.Debug("Z Axis Move to Pick Position");
                    ZAxis.MoveAbs(_recipeList.TransferRecipe.ZAxisPlacePosition);
#if SIMULATION
                    Wait(100);
#else
                    Wait((int)(_recipeList.CommonRecipe.MotionMoveTimeOut * 1000),
                        () => ZAxis.IsOnPosition(_recipeList.TransferRecipe.PickZ));
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.ZAxis_MovePickPosition_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferZAxis_PickTimeout);
                        return;
                    }
#endif
                    Log.Debug("Z Axis Move Pick Position Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Cylinders1_Down:
                    if (_recipeList.CommonRecipe.SkipCylinder1) 
                    { 
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("Cylinder1 Down");
                    _devices.Outputs.Cylinder1Down.Value = true;
                    Wait(100);
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Cylinders1_Down_Wait:
                    if (_recipeList.CommonRecipe.SkipCylinder1)
                    {
                        Step.RunStep++;
                        break;
                    }
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferCylinder_Pick1Timeout);
                        return;
                    }
#endif
                    Log.Debug("Cylinder1 Down Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Vacuum1_On:
                    if (_recipeList.CommonRecipe.SkipCylinder1)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("Vacuum1 On");
                    _devices.Outputs.Vacuum1On.Value = true;
                    Wait(100);
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Vacuum1_On_Wait:
                    if (_recipeList.CommonRecipe.SkipCylinder1)
                    {
                        Step.RunStep++;
                        break;
                    }
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.Vacuum1_OnTimeout);
                        return;
                    }
#endif
                    Log.Debug("Vacuum1 On Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Cylinders1_Up:
                    if (_recipeList.CommonRecipe.SkipCylinder1)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("Cylinder1 Up");
                    _devices.Outputs.Cylinder1Up.Value = true;
                    Wait(100);
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Cylinders1_Up_Wait:
                    if (_recipeList.CommonRecipe.SkipCylinder1)
                    {
                        Step.RunStep++;
                        break;
                    }
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferCylinder_Pick1Timeout);
                        return;
                    }
#endif
                    Log.Debug("Cylinder1 Up Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Cylinders2_Down:
                    if (_recipeList.CommonRecipe.SkipCylinder2)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("Cylinder2 Down");
                    _devices.Outputs.Cylinder2Down.Value = true;
                    Wait(100);
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Cylinders2_Down_Wait:
                    if (_recipeList.CommonRecipe.SkipCylinder2)
                    {
                        Step.RunStep++;
                        break;
                    }
#if !SIMULATION
                    if (WaitTimeOutOccurred) 
                    {
                        RaiseAlarm(EAlarm.TransferCylinder_Pick2Timeout);
                        return;
                    }
#endif
                    Log.Debug("Cylinder2 Down Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Vacuum2_On:
                    if (_recipeList.CommonRecipe.SkipCylinder2)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("Vaccum 2 On");
                    _devices.Outputs.Vacuum2On.Value = true;
                    Wait(100);
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Vacuum2_On_Wait:
                    if (_recipeList.CommonRecipe.SkipCylinder2)
                    {
                        Step.RunStep++;
                        break;
                    }
#if !SIMULATION 
                    if (WaitTimeOutOccurred)
                    { 
                        RaiseAlarm((int)EAlarm.Vacuum2_OnTimeout);
                        return;
                    }
#endif
                    Log.Debug("Vaccum 2 On Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Cylinders2_Up:
                    if (_recipeList.CommonRecipe.SkipCylinder2)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("Cylinder2 Up");
                    _devices.Outputs.Cylinder2Up.Value = true;
                    Wait(100);
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Cylinders2_Up_Wait:
                    if (_recipeList.CommonRecipe.SkipCylinder2)
                    {
                        Step.RunStep++;
                        break;
                    }
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferCylinder_Pick2Timeout);
                        return;
                    }
#endif
                    Log.Debug("Cylinder2 Up Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.XAxis_MoveInspectVision1:
                    Log.Debug("X Axis Move To Inspect Vision 1");
                    XAxis.MoveAbs(_recipeList.TransferRecipe.XAxisInspectPosition1);
                    Wait((int)(_recipeList.CommonRecipe.MotionMoveTimeOut * 1000),
                        () => XAxis.IsOnPosition(_recipeList.TransferRecipe.XAxisInspectPosition1));
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.XAxis_MoveInspectVision1_Wait:
#if !SIMULATION
                    if(WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.XAxis_MoveInspectPosition_TimeOut);
                        break;
                    }
#endif
                    Log.Debug("X Axis Move Inspect Position 1 Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.YAxis_MoveInspectVision1:
                    Log.Debug("Y Axis Move To Inspect Vision");
                    YAxis.MoveAbs(_recipeList.TransferRecipe.YAxisInspectionPosition1);
                    Wait((int)(_recipeList.CommonRecipe.MotionMoveTimeOut * 1000),
                        () => YAxis.IsOnPosition(_recipeList.TransferRecipe.YAxisInspectionPosition1));
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.YAxis_MoveInspectVision1_Wait:
#if !SIMULATION
                    if(WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.YAxis_MoveInspectPosition_TimeOut);
                        break;
                    }
#endif
                    Log.Debug("Y Axis Move Inspect Position 1 Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.ZAxis_MoveInspectVision1:
                    Log.Debug("Z Axis Move Inspect Position 1");
                    ZAxis.MoveAbs(_recipeList.TransferRecipe.ZAxisInspectionPosition1);
                    Wait((int)(_recipeList.CommonRecipe.MotionMoveTimeOut * 1000),
                        () => ZAxis.IsOnPosition(_recipeList.TransferRecipe.ZAxisInspectionPosition1));
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.ZAxis_MoveInspectVision1_Wait:
#if !SIMULATION
                    if(WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.ZAxis_MoveInspectPosition_TimeOut);
                        break;
                    }
#endif
                    Log.Debug("Z Axis Move Inspect Position 1 Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.XAxis_MoveInspectVision2:
                    Log.Debug("X Axis Move To Inspect Vision 2");
                    XAxis.MoveAbs(_recipeList.TransferRecipe.XAxisInspectPosition2);
                    Wait((int)(_recipeList.CommonRecipe.MotionMoveTimeOut * 1000),
                        () => XAxis.IsOnPosition(_recipeList.TransferRecipe.XAxisInspectPosition2));
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.XAxis_MoveInspectVision2_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.XAxis_MoveInspectPosition_TimeOut);
                        break;
                    }
#endif
                    Log.Debug("X Axis Move Inspect Position 2 Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.YAxis_MoveInspectVision2:
                    Log.Debug("Y Axis Move To Inspect Vision 2");
                    YAxis.MoveAbs(_recipeList.TransferRecipe.YAxisInspectionPosition2);
                    Wait((int)(_recipeList.CommonRecipe.MotionMoveTimeOut * 1000),
                        () => YAxis.IsOnPosition(_recipeList.TransferRecipe.YAxisInspectionPosition2));
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.YAxis_MoveInspectVision2_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.YAxis_MoveInspectPosition_TimeOut);
                        break;
                    }
#endif
                    Log.Debug("Y Axis Move Inspect Position 2 Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.ZAxis_MoveInspectVision2:
                    Log.Debug("Z Axis Move Inspect Position");
                    ZAxis.MoveAbs(_recipeList.TransferRecipe.ZAxisInspectionPosition2);
                    Wait((int)(_recipeList.CommonRecipe.MotionMoveTimeOut * 1000),
                        () => ZAxis.IsOnPosition(_recipeList.TransferRecipe.ZAxisInspectionPosition2));
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.ZAxis_MoveInspectVision2_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.ZAxis_MoveInspectPosition_TimeOut);
                        break;
                    }
#endif
                    Log.Debug("Z Axis Move Inspect Position 2 Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.IfSkipVision:
                    if(!_recipeList.CommonRecipe.SkipVisionCheck)
                    {
                        FlagRequestVisionInspect1 = true;
                        FlagRequestVisionInspect2 = true;
                        Sequence = ESequence.Inspect;
                        Log.Debug("Vision Check");
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.End:
                    Log.Debug("Pick End");
                    Sequence = ESequence.Place;
                    break;
            }
        }

        private void Sequence_Place()
        {
            switch ((ETransferProcessPlaceStep)Step.RunStep)
            {
                case ETransferProcessPlaceStep.Start:
                    Log.Debug("Place Start");
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.XAxis_MovePlacePosition:
                    Log.Debug("X Axis Move to Place Position");
                    XAxis.MoveAbs(_recipeList.TransferRecipe.XAxisPlacePosition);
#if SIMULATION
                    Wait(100);
#else
                    Wait((int)(_recipeList.CommonRecipe.MotionMoveTimeOut * 1000),
                        () => XAxis.IsOnPosition(_recipeList.TransferRecipe.PlaceX));
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.XAxis_MovePlacePosition_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferXAxis_PlaceTimeout);
                        return;
                    }
#endif
                    Log.Debug("X Axis Move Place Position Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.YAxis_MovePlacePosition:
                    Log.Debug("Y Axis Move to Place Position");
                    YAxis.MoveAbs(_recipeList.TransferRecipe.YAxisPlacePosition);
#if SIMULATION
                    Wait(100);
#else
                    Wait((int)(_recipeList.CommonRecipe.MotionMoveTimeOut * 1000),
                        () => YAxis.IsOnPosition(_recipeList.TransferRecipe.PlaceY));
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.YAxis_MovePlacePosition_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferYAxis_PlaceTimeout);
                        return;
                    }
#endif
                    Log.Debug("Y Axis Move Place Position Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.ZAxis_MovePlacePosition:
                    Log.Debug("Z Axis Move to Place Position");
                    ZAxis.MoveAbs(_recipeList.TransferRecipe.ZAxisPlacePosition);
#if SIMULATION
                    Wait(100);
#else
                    Wait((int)(_recipeList.CommonRecipe.MotionMoveTimeOut * 1000),
                        () => ZAxis.IsOnPosition(_recipeList.TransferRecipe.PlaceZ));
#endif
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.ZAxis_MovePlacePosition_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferZAxis_PlaceTimeout);
                        return;
                    }
#endif
                    Log.Debug("Z Axis Move Place Position Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.Cylinders1_Down:
                    if (_recipeList.CommonRecipe.SkipCylinder1)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("Cylinder 1 Down");
                    _devices.Outputs.Cylinder1Down.Value = true;
                    Wait(100);
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.Cylinders1_Down_Wait:
                    if (_recipeList.CommonRecipe.SkipCylinder1)
                    {
                        Step.RunStep++;
                        break;
                    }
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferCylinder_Place1Timeout);
                        return;
                    }
#endif
                    Log.Debug("Cylinder 1 Down Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.Vacuum1_Blow:
                    if (_recipeList.CommonRecipe.SkipCylinder1)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("Vaccum 1 Blow On");
                    _devices.Outputs.Vacuum1Blow.Value = true;
                    Wait(100);
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.Vacuum1_Blow_Wait:
                    if (_recipeList.CommonRecipe.SkipCylinder1)
                    {
                        Step.RunStep++;
                        break;
                    }
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.Vacuum1_BlowTimeout);
                        return;
                    }
#endif
                    Log.Debug("Vaccum Blow On Done");
                    Wait(100);
                    _devices.Outputs.Vacuum1Blow.Value = false;
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.Cylinders1_Up:
                    if (_recipeList.CommonRecipe.SkipCylinder1)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("Cylinder 1 Up");
                    _devices.Outputs.Cylinder1Up.Value = true;
                    Wait(100);
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.Cylinders1_Up_Wait:
                    if (_recipeList.CommonRecipe.SkipCylinder1)
                    {
                        Step.RunStep++;
                        break;
                    }
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferCylinder_Place1Timeout);
                        return;
                    }
#endif
                    Log.Debug("Cylinder 1 Up Done");
                    _devices.Outputs.Vacuum1Blow.Value = false;
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.Cylinders2_Down:
                    if (_recipeList.CommonRecipe.SkipCylinder2)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("Cylinder 2 Down");
                    _devices.Outputs.Cylinder2Down.Value = true;
                    Wait(100);
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.Cylinders2_Down_Wait:
                    if (_recipeList.CommonRecipe.SkipCylinder2)
                    {
                        Step.RunStep++;
                        break;
                    }
#if !SIMULATION
 
                    if(WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferCylinder_Place2Timeout);
                        return;
                    }
#endif
                    Log.Debug("Cylinder 2 Down Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.Vacuum2_Blow:
                    if (_recipeList.CommonRecipe.SkipCylinder2)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("Vaccum 2 Blow");
                    _devices.Outputs.Vacuum2Blow.Value = true;
                    Wait(100);
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.Vacuum2_Blow_Wait:
                    if (_recipeList.CommonRecipe.SkipCylinder2)
                    {
                        Step.RunStep++;
                        break;
                    }
#if !SIMULATION
                    if(WaitTimeOutOccurred)
                    { 
                        RaiseAlarm(EAlarm.Vacuum2_BlowTimeout);
                        return;
                    }
#endif
                    Log.Debug("Vaccum 2 Blow Done");
                    Wait(100);
                    _devices.Outputs.Vacuum2Blow.Value = false;
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.Cylinders2_Up:
                    if (_recipeList.CommonRecipe.SkipCylinder2)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("Cylinder 2 Up");
                    _devices.Outputs.Cylinder2Up.Value = true;
                    Wait(100);
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.Cylinders2_Up_Wait:
                    if (_recipeList.CommonRecipe.SkipCylinder2)
                    {
                        Step.RunStep++;
                        break;
                    }
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.Vacuum2_BlowTimeout);
                        return;
                    }
#endif
                    Log.Debug("Cylinder 2 Up Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.End:
                    Log.Debug("Place End");
                    Sequence = ESequence.AutoRun;
                    break;
            }
        }
#endregion
    }
}
