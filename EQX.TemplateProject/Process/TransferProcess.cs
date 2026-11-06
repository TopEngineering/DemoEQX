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

        //private bool FlagIn_ShuttleRequestLoad
        //{
        //    get
        //    {
        //        return VirtualInput[ETransferProcessInput.SHUTTLE_REQUEST_LOAD].Value;
        //    }
        //}

        //private bool FlagOut_TransferLoadDone
        //{
        //    set
        //    {
        //        VirtualOutput[ETransferProcessOutput.TRANSFER_LOAD_DONE].Value = value;
        //    }
        //}
        #endregion

        #region Constructor
        public TransferProcess(Devices devices,
            VirtualIO virtualIO,
            RecipeList recipeList)
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
                    Sequence = ESequence.Stop;
                    break;
                case ESequence.Pick:
                    Sequence_Pick();
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
            Sequence = ESequence.Pick;
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
                    XAxis.MoveAbs(_recipeList.TransferRecipe.PickX);
                    Wait(1000);
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.XAxis_MovePickPosition_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferXAxis_PickTimeout);
                    }
                    Log.Debug("X Axis Move Pick Position Done");
                    break;
                case ETransferProcessPickStep.YAxis_MovePickPosition:
                    Log.Debug("Y Axis Move to Pick Position");
                    YAxis.MoveAbs(_recipeList.TransferRecipe.PickY);
                    Wait(1000);
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.YAxis_MovePickPosition_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferYAxis_PickTimeout);
                    }
                    Log.Debug("Y Axis Move Pick Position Done");
                    break;
                case ETransferProcessPickStep.ZAxis_MovePickPosition:
                    Log.Debug("Z Axis Move to Pick Position");
                    ZAxis.MoveAbs(_recipeList.TransferRecipe.PickZ);
                    Wait(1000);
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.ZAxis_MovePickPosition_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferZAxis_PickTimeout);
                    }
                    Log.Debug("Z Axis Move Pick Position Done");
                    break;
                case ETransferProcessPickStep.Cylinders1_Down:
                    Log.Debug("Cylinder1 Down");
                    Wait(1000);
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Cylinders1_Down_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferCylinder_Pick1Timeout);
                    }
                    Log.Debug("Cylinder1 Down Done");
                    break;
                case ETransferProcessPickStep.Vacuum1_On:
                    Log.Debug("Vacuum1 On");
                    Wait(1000);
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Vacuum1_On_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.Vacuum1_OnTimeout);
                    }
                    Log.Debug("Vacuum1 On Done");
                    break;
                case ETransferProcessPickStep.Cylinders1_Up:
                    Log.Debug("Cylinder1 Up");
                    Wait(1000);
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Cylinders1_Up_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferCylinder_Place1Timeout);
                    }
                    Log.Debug("Cylinder1 Up Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Cylinders2_Down:
                    Log.Debug("Cylinder2 Down");
                    Wait(1000);
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Cylinders2_Down_Wait:
                    if (WaitTimeOutOccurred) 
                    {
                        RaiseAlarm(EAlarm.TransferCylinder_Pick2Timeout);
                    }
                    Log.Debug("Cylinder2 Down Done");
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Cylinders2_Up:
                    Log.Debug("Cylinder2 Up");
                    Wait(1000);
                    Step.RunStep++;
                    break;
                case ETransferProcessPickStep.Cylinders2_Up_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferCylinder_Place2Timeout);
                    }
                    Log.Debug("Cylinder2 Up Done");
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
                    XAxis.MoveAbs(_recipeList.TransferRecipe.PlaceX);
                    Wait(1000);
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.XAxis_MovePlacePosition_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferXAxis_PlaceTimeout);
                    }
                    Log.Debug("X Axis Move Place Position Done");
                    break;
                case ETransferProcessPlaceStep.YAxis_MovePlacePosition:
                    Log.Debug("Y Axis Move to Place Position");
                    YAxis.MoveAbs(_recipeList.TransferRecipe.PlaceY);
                    Wait(1000);
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.YAxis_MovePlacePosition_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferYAxis_PlaceTimeout);
                    }
                    Log.Debug("Y Axis Move Place Position Done");
                    break;
                case ETransferProcessPlaceStep.ZAxis_MovePlacePosition:
                    Log.Debug("Z Axis Move to Place Position");
                    ZAxis.MoveAbs(_recipeList.TransferRecipe.PlaceZ);
                    Wait(1000);
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.ZAxis_MovePlacePosition_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferZAxis_PlaceTimeout);
                    }
                    Log.Debug("Z Axis Move Place Position Done");
                    break;
                case ETransferProcessPlaceStep.Cylinders_Down:
                    Log.Debug("Cylinder Down");
                    Cylinder1.Backward();
                    Wait(1000);
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.Cylinders_Down_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferCylinder_Place1Timeout);
                    }
                    _devices.Outputs.Vacuum1On.Value = false;
                    _devices.Outputs.Vacuum1Blow.Value = false;
                    Log.Debug("Cylinder Down Done");
                    break;
                case ETransferProcessPlaceStep.Cylinders_Up:
                    Log.Debug("Cylinder Up");
                    Wait(1000);
                    Step.RunStep++;
                    break;
                case ETransferProcessPlaceStep.Cylinders_Up_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferCylinder_Place1Timeout);
                    }
                    Log.Debug("Cylinder Up Done");
                    break;
                case ETransferProcessPlaceStep.End:
                    Log.Debug("Place End");
                    Sequence = ESequence.Pick;
                    break;
            }
        }
        #endregion
    }
}
