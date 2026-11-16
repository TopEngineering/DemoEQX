using EQX.Core.InOut;
using EQX.Process;
using EQX.Core.Sequence;
using EQX.InOut.Virtual;
using EQX.TemplateProject.Defines.Process.Step.VisionProcess;
using System.Diagnostics;
using EQX.TemplateProject.Defines;
using EQX.TemplateProject.Defines.Devices;
using EQX.TemplateProject.Recipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.TemplateProject.Process
{
    public class VisionProcess : ProcessBase<ESequence>
    {
        #region private
        private readonly Devices _devices;
        private readonly VirtualIO _virtualIO;
        private readonly RecipeList _recipeList;
        private readonly Stopwatch _requestResetTimer = new();

        #endregion
        #region Flags
        private IDInputDevice VirtualInput => _virtualIO.VisionProcessInput;
        private IDOutputDevice VirtualOutput => _virtualIO.VisionProcessOutput;
        private bool FlagVisionCheckDone
        {
            set { VirtualOutput[(int)EVisionProcessOutput.VISION_CHECK_DONE] = value; }
        }
        private bool FlagTransferRequestVisionCheck
        {
            get { return VirtualInput[(int)EVisionProcessInput.TRANSFER_REQUEST_VISION_CHECK]; }
        }

        #endregion
        #region Constructors
        public VisionProcess(Devices devices, VirtualIO virtualIO, RecipeList recipeList)
        {
            _devices = devices;
            _virtualIO = virtualIO;
            _recipeList = recipeList;
        }
        #endregion
        #region Override Methods
        public override bool ProcessOrigin()
        {
            switch((EVisionProcessOriginStep)Step.OriginStep)
            {
                case EVisionProcessOriginStep.Start:
                    Log.Debug("VisionProcess Origin Start");
                    Step.OriginStep++;
                    break;
                case EVisionProcessOriginStep.End:
                    ProcessStatus = EProcessStatus.OriginDone;
                    Log.Debug("Vision Origin Done");
                    Step.OriginStep++;
                    break;
            }
            return true;
        }
        public override bool ProcessToRun()
        {
            return base.ProcessToRun();
        }
        public override bool ProcessRun()
        {
            switch(Sequence)
            {
                case ESequence.Stop:
                    Wait(50);
                    break;
                case ESequence.Ready:
                    Sequence_Ready();
                    break;
                case ESequence.AutoRun:
                    Sequence_AutoRun();
                    break;
                case ESequence.Inspect:
                    Sequence_VisionCheck();
                    break;
            }
            return true;
        }
        #endregion
        #region Sequences
        private void Sequence_Ready() 
        { 
            switch((EVisionProcessReadyStep)Step.RunStep)
            {
                case EVisionProcessReadyStep.Start:
                    Log.Debug("Vision Ready");
                    Step.RunStep++;
                    break;
                case EVisionProcessReadyStep.CheckCameraConnection:
                    Log.Debug("Check Camera Connection");
                    Wait(100);
                    Step.RunStep++;
                    break;
                case EVisionProcessReadyStep.End:
                    Log.Debug("Vision Ready Done");
                    Sequence = ESequence.Stop;
                    break;
            }
        }
        private void Sequence_AutoRun() 
        {
        switch((EVisionProcessAutoRunStep)Step.RunStep)
            {
                case EVisionProcessAutoRunStep.Start:
                    Log.Debug("Vision AutoRun Start");
                    Step.RunStep++;
                    break;
                case EVisionProcessAutoRunStep.WaitTransferRequest:
                    Log.Debug("Transfer Request");
                    Step.RunStep++;
                    break;
                case EVisionProcessAutoRunStep.End:
                    Sequence = ESequence.Inspect;
                    break;
            }
        }
        private void Sequence_VisionCheck() 
        {
        switch((EVisionProcessCheckStep)Step.RunStep)
            {
                case EVisionProcessCheckStep.Start:
                    Log.Debug("Vision check start");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.SetCurrentHead:
                    Log.Debug("Set Current Head");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.Cylinder_Move_VisionPosition:
                    Log.Debug("Cylinder Move Vision Position");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.Cylinder_Move_VisionPosition_Wait:
                    Log.Debug("Cylinder Move Vision Position Done");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.ImageGrab:
                    Log.Debug("Image Grab");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.ImageGrab_Wait:
                    Log.Debug("Image Grab Done");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.VisionInspect:
                    Log.Debug("Vision Inspect");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.VisionInspect_Wait:
                    Log.Debug("Vision Inspect Done");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.ResultUpdate:
                    Log.Debug("Result Update");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.UpdateCurrentHead:
                    Log.Debug("Update Current Head");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.Vision_CheckDone:
                    Log.Debug("Vision Check Done");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.Vision_ClearFlags:
                    Log.Debug("Vision Clear Flag");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.End:
                    Log.Debug("Vision Check Done");
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    break;
            }

        }
        #endregion
    }
}
