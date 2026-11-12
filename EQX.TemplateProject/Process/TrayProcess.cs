using EQX.Core.InOut;
using EQX.Core.Sequence;
using EQX.Process;
using EQX.TemplateProject.Defines;
using EQX.TemplateProject.Defines.Devices;
using EQX.TemplateProject.Defines.Process.Step.TrayProcess;
using EQX.TemplateProject.Recipe;
using ScottPlot.Plottables;
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
        private readonly IDInputDevice _trayInput;
        private readonly IDOutputDevice _trayOutput;
        #endregion
        #region Flags
        private IDInputDevice VirtualInput => _virtualIO.TrayProcessInput;
        private IDOutputDevice VirtualOutput => _virtualIO.TrayProcessOutput;
        private bool FlagTrayPickCellReady
        {
            set { _trayOutput[(int)ETrayProcessOutput.TRAY_PICK_CELL_READY] = value; }
        }
        private bool FlagTrayPlaceCellReady
        {
            set { _trayOutput[(int)ETrayProcessOutput.TRAY_PLACE_CELL_READY] = value; }
        }
        private bool FlagTrayPickDone
        {
            set { _trayOutput[(int)ETrayProcessOutput.TRAY_PICK_DONE] = value; }
        }
        private bool FlagTrayPlaceDone
        {
            set { _trayOutput[(int)ETrayProcessOutput.TRAY_PLACE_DONE] = value; }
        }
        private bool FlagTransferRequestPickCell
        {
            get { return _trayInput[(int)ETrayProcessInput.TRANSFER_REQUEST_PICK_CELL]; }
        }
        private bool FlagTransferRequestPlaceCell
        {
            get { return _trayInput[(int)ETrayProcessInput.TRANSFER_REQUEST_PLACE_CELL]; }
        }
        private bool FlagTransferPickDone
        {
            get { return _trayInput[(int)ETrayProcessInput.TRANSFER_PICK_DONE]; }
        }
        private bool FlagTransferPlaceDone
        {
            get { return _trayInput[(int)ETrayProcessInput.TRANSFER_PLACE_DONE]; }
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
                    Step.OriginStep++;
                    break;
                case ETrayProcessOriginStep.End:
                    Log.Debug("Tray Origin Done");
                    ProcessStatus = EProcessStatus.OriginDone;
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
            switch((ETrayProcessAutoRunStep)Step.RunStep)
            { 
                case ETrayProcessAutoRunStep.Start:
                    Log.Debug("Tray AutoRun Start");
                    Step.RunStep++;
                    break;
                case ETrayProcessAutoRunStep.CheckTrayStatus:
                    Log.Debug("CheckTray");
                    Step.RunStep++;
                    break;
                case ETrayProcessAutoRunStep.WaitTransferRequest:
                    Log.Debug("Wait Transfer Request");
                    Step.RunStep++;
                    break;
                case ETrayProcessAutoRunStep.End:
                    Log.Debug("Tray AutoRun Done");
                    Sequence = ESequence.CheckTray;
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
                    Log.Debug("Check Input Tray");
                    Step.RunStep++;
                    break;
                case ETrayProcessCheckTrayStep.InputTrayCheck_Wait:
                    Log.Debug("Check Tray Input Done");
                    Step.RunStep++;
                    break;
                case ETrayProcessCheckTrayStep.OutputTrayCheck:
                    Log.Debug("Check Output Tray");
                    Step.RunStep++;
                    break;
                case ETrayProcessCheckTrayStep.OutputTrayCheck_Wait:
                    Log.Debug("Check Output Tray Done");
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
                case ETrayProcessPreparePickCellStep.TransferRequestPick:
                    Log.Debug("Transfer Resquest Pick");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePickCellStep.TrayPickCellReady:
                    Log.Debug("Tray Pick Cell Ready");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePickCellStep.TransferPickDone:
                    Log.Debug("Transfer Pick Done");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePickCellStep.TrayPickDone:
                    Log.Debug("Tray Pick Done");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePickCellStep.UpdateInputIndex:
                    Log.Debug("Update Input Index");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePickCellStep.ClearPickFlags:
                    Log.Debug("Clear Pick Flag");
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
                    Log.Debug("Check Output Tray");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePlaceCellStep.TransferRequestPlace:
                    Log.Debug("Transfer Request Place");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePlaceCellStep.TrayPlaceCellReady:
                    Log.Debug("Tray Place Cell Ready");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePlaceCellStep.TransferPlaceDone:
                    Log.Debug("Transfer Place Done");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePlaceCellStep.UpdateOutputIndex:
                    Log.Debug("Update Output Index");
                    Step.RunStep++;
                    break;
                case ETrayProcessPreparePlaceCellStep.ClearPlaceFlags:
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
