using EQX.Core.InOut;
using EQX.Core.Sequence;
using EQX.InOut.Virtual;
using EQX.Process;
using EQX.TemplateProject.Defines;
using EQX.TemplateProject.Defines.Devices;
using EQX.TemplateProject.Defines.Process.Step.TrayProcess;
using EQX.TemplateProject.Recipe;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.TemplateProject.Process
{
    public class TrayProcess : ProcessBase<ESequence>
    {
        #region private
        private readonly Devices _devices;
        private readonly VirtualIO _virtualIO;
        private readonly RecipeList _recipeList;
        private int InputIndex;
        private int OutputIndex;
        #endregion
        #region Flags
        private IDInputDevice VirtualInput => _virtualIO.TrayProcessInput;
        private IDOutputDevice VirtualOutput => _virtualIO.TrayProcessOutput;
        private bool FlagTrayPickCellReady
        {
            set { VirtualOutput[(int)ETrayProcessOutput.TRAY_PICK_CELL_READY] = value; }
        }
        private bool FlagTrayPlaceCellReady
        {
            set { VirtualOutput[(int)ETrayProcessOutput.TRAY_PLACE_CELL_READY] = value; }
        }
        private bool FlagTrayPickDone
        {
            set { VirtualOutput[(int)ETrayProcessOutput.TRAY_PICK_DONE] = value; }
        }
        private bool FlagTrayPlaceDone
        {
            set { VirtualOutput[(int)ETrayProcessOutput.TRAY_PLACE_DONE] = value; }
        }
        private bool FlagTransferRequestPickCell
        {
            get { return VirtualInput[(int)ETrayProcessInput.TRANSFER_REQUEST_PICK_CELL]; }
        }
        private bool FlagTransferRequestPlaceCell
        {
            get { return VirtualInput[(int)ETrayProcessInput.TRANSFER_REQUEST_PLACE_CELL]; }
        }
        private bool FlagTransferPickDone
        {
            get { return VirtualInput[(int)ETrayProcessInput.TRANSFER_PICK_DONE]; }
        }
        private bool FlagTransferPlaceDone
        {
            get { return VirtualInput[(int)ETrayProcessInput.TRANSFER_PLACE_DONE]; }
        }
        #endregion
        #region Constructors
        public TrayProcess(Devices devices, VirtualIO virtualIO, RecipeList recipeList)
        {
            _devices = devices;
            _virtualIO = virtualIO;
            _recipeList = recipeList;
        }

        #endregion
        #region Override Methods
        public override bool ProcessToRun()
        {
            return base.ProcessToRun();
        }
        public override bool ProcessOrigin()
        {
            switch((ETrayProcessOriginStep)Step.OriginStep)
            {
                case ETrayProcessOriginStep.Start:
                    Log.Debug("Tray Origin Start");
                    Step.OriginStep++;
                    break;
                case ETrayProcessOriginStep.ClearFlags:
                    Log.Debug("Clear Flags");
                    ((MappableOutputDevice<ETrayProcessOutput>)VirtualOutput).ClearOutputs();
                    Step.OriginStep++;
                    break;
                case ETrayProcessOriginStep.End:
                    Log.Debug("Tray Origin Done");
                    ProcessStatus = EProcessStatus.OriginDone;
                    Step.OriginStep++;
                    break;
            }
            return true;
        }
        public override bool ProcessRun()
        {
            switch(Sequence)
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
                case ESequence.CheckTray:
                    Sequence_CheckTray();
                    break;
                case ESequence.PreparePickCell:
                    Sequence_PreparePickCell();
                    break;
                case ESequence.PreparePlaceCell:
                    Sequence_PreparePlaceCell();
                    break;
            }
            return true;
        }
        #endregion
        #region Sequences
        private void Sequence_AutoRun()
        {
            switch ((ETrayProcessAutoRunStep)Step.RunStep)
            {
                case ETrayProcessAutoRunStep.Start:
                    Log.Debug("Tray AutoRun Start");
                    Step.RunStep++;
                    break;
                case ETrayProcessAutoRunStep.CheckTrayStatus:
                    Log.Debug("CheckTray");
                    break;
                case ETrayProcessAutoRunStep.WaitTransferRequest:
                    if (FlagTransferRequestPickCell)
                        Sequence = ESequence.PreparePickCell;
                    else if (FlagTransferRequestPlaceCell)
                        Sequence = ESequence.PreparePlaceCell;
                    else
                        Wait(50);
                    break;
                case ETrayProcessAutoRunStep.End:
                    Sequence = ESequence.Stop;
                    break;
            }
        }
        private void Sequence_Ready()
        {
            switch((ETrayProcessReadyStep)Step.RunStep)
            {
                case ETrayProcessReadyStep.Start:
                    Log.Debug("Ready Start");
                    Step.RunStep++;
                    break;
                case ETrayProcessReadyStep.ClearFlags:
                    Log.Debug("Clear Flag");
                    ((MappableOutputDevice<ETrayProcessOutput>)VirtualOutput).ClearOutputs(); 
                    Step.RunStep++;
                    break;
                case ETrayProcessReadyStep.CheckTrayRecipe:
                    Log.Debug("Check Tray Recipe");
                    Step.RunStep++;
                    break;
                case ETrayProcessReadyStep.CheckTransferReady:
                    Log.Debug("Check Transfer Ready");
                    Step.RunStep++;
                    break;
                case ETrayProcessReadyStep.End:
                    Log.Debug("Tray Ready Done");
                    Sequence = ESequence.Stop;
                    break;
            }
        }
        private void Sequence_CheckTray()
        { 
            switch((ETrayProcessCheckTrayStep)Step.RunStep)
            {
                case ETrayProcessCheckTrayStep.Start:
                    Log.Debug("Check Tray Start");
                    Step.RunStep++;
                    break;
                case ETrayProcessCheckTrayStep.InputTrayCheck:
                    Log.Debug("Check Input Tray Presence");
#if SIMULATION
                    Wait(100);
#else
                    Wait((int)(_recipeList.CommonRecipe.MaterialInputTimeout * 1000),
                        () => _devices.Inputs.InputTrayDetect.Value);
#endif
                    Step.RunStep++;
                    break;
                case ETrayProcessCheckTrayStep.InputTrayCheck_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferInputTrayTimeout);
                        return;
                    }
#endif
                    Log.Debug("Check Tray Input Done");
                    Step.RunStep++;
                    break;
                case ETrayProcessCheckTrayStep.OutputTrayCheck:
                    Log.Debug("Check Output Tray Presence");
#if SIMULATION
                    Wait(100);
#else
                    Wait((int)(_recipeList.CommonRecipe.MaterialInputTimeout * 1000),
                        () => _devices.Inputs.OutputTrayDetect.Value);
#endif
                    Step.RunStep++;
                    break;
                case ETrayProcessCheckTrayStep.OutputTrayCheck_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.TransferOutputTrayTimeout);
                        return;
                    }
#endif
                    Log.Debug("Check Output Tray Done");
                    Step.RunStep++;
                    break;
                case ETrayProcessCheckTrayStep.End:
                    Log.Debug("Check Tray Done");
                    Sequence = ESequence.PreparePickCell;
                    break;
            }
        }
        private void Sequence_PreparePickCell()
        { 
            switch((ETrayProcessPreparePickCellStep)Step.RunStep)
            {
                case ETrayProcessPreparePickCellStep.Start:
                    Log.Debug("Pick Start");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePickCellStep.CheckInputTray:
                    if (InputIndex < (long)_recipeList.TrayRecipe.InputTrayRow * _recipeList.TrayRecipe.InputTrayColumn)
                    {
                        RaiseAlarm(EAlarm.TrayInputEmpty);
                        return;
                    }
#if !SIMULATION
                    if (!_devices.Inputs.InputTrayDetect.Value)
                    {
                        RaiseAlarm(EAlarm.TransferInputTrayTimeout);
                        return;
                    }
#endif
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePickCellStep.TransferRequestPick:
                    if(FlagTransferRequestPickCell == false)
                    {
                        RaiseAlarm(EAlarm.TransferPickFailed);
                        Wait(50);
                        break;
                    }
                    Log.Debug("Transfer Resquest Pick");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePickCellStep.TrayPickCellReady:
                    FlagTrayPickCellReady = true;
                    Log.Debug("Tray Pick Cell Ready");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePickCellStep.TransferPickDone:
                    if (FlagTransferPickDone == false)
                    {
                        RaiseAlarm(EAlarm.TransferPickFailed);
                        break;
                    }
                    Log.Debug("Transfer Pick Done");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePickCellStep.TrayPickDone:
                    FlagTrayPickDone = true;
                    Log.Debug("Tray Pick Done");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePickCellStep.UpdateInputIndex:
                    Log.Debug("Update Input Index");
                    InputIndex++;
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePickCellStep.ClearPickFlags:
                    ((MappableOutputDevice<ETrayProcessOutput>)VirtualOutput).ClearOutputs(); 
                    Log.Debug("Clear Pick Flags");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePickCellStep.End:
                    Log.Debug("Pick Done");
                    Sequence = ESequence.PreparePlaceCell;
                    break;
            }
        }
        private void Sequence_PreparePlaceCell()
        { 
            switch((ETrayProcessPreparePlaceCellStep)Step.RunStep)
            {
                case ETrayProcessPreparePlaceCellStep.Start:
                    Log.Debug("Place Start");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePlaceCellStep.CheckOutputTray:
                    if (OutputIndex >= (long)_recipeList.TrayRecipe.OutputTrayRow * _recipeList.TrayRecipe.OutputTrayColumn)
                    {
                        RaiseAlarm(EAlarm.TrayOutputFull);
                        return;
                    }
#if !SIMULATION
                    if (!_devices.Inputs.OutputTrayDetect.Value)
                    {
                        RaiseAlarm(EAlarm.TransferOutputTrayTimeout);
                        return;
                    }
#endif
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePlaceCellStep.TransferRequestPlace:
                    Log.Debug("Transfer Request Place");
                    if(FlagTransferRequestPlaceCell == false)
                    {
                        RaiseAlarm(EAlarm.TransferRequestPlaceFailed);
                        Wait(50);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePlaceCellStep.TrayPlaceCellReady:
                    FlagTrayPlaceCellReady = true;
                    Log.Debug("Tray Place Cell Ready");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePlaceCellStep.TransferPlaceDone:
                    if(FlagTransferPlaceDone == false)
                    {
                        RaiseAlarm(EAlarm.TransferPlaceFailed);
                        Wait(50);
                        break;
                    }
                    Log.Debug("Transfer Place Done");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePlaceCellStep.TrayPlaceDone:
                    FlagTrayPlaceDone = true;
                    Log.Debug("Tray Place Done");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePlaceCellStep.UpdateOutputIndex:
                    Log.Debug("Update Output Index");
                    OutputIndex++;
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePlaceCellStep.ClearPlaceFlags:
                    ((MappableOutputDevice<ETrayProcessOutput>)VirtualOutput).ClearOutputs(); FlagTrayPlaceCellReady = false;
                    FlagTrayPlaceDone = false;
                    Log.Debug("Clear Place Flag");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePlaceCellStep.End:
                    Log.Debug("Place Done");
                    Sequence = ESequence.PreparePickCell;
                    break;
            }
        }
       #endregion
    }
    
}
