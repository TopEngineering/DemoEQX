using EQX.Core.Common;
using EQX.Core.InOut;
using EQX.Process;
using EQX.Core.Sequence;
using EQX.Core.Vision;
using EQX.Core.Vision.Algorithms;
using EQX.Core.Vision.Grabber;
using EQX.InOut.Virtual;
using EQX.TemplateProject.Defines.Process.Step.VisionProcess;
using System.Diagnostics;
using EQX.TemplateProject.Defines;
using EQX.TemplateProject.Defines.Cylinder;
using EQX.TemplateProject.Defines.Devices;
using EQX.TemplateProject.Defines.Devices.Cylinder;
using EQX.TemplateProject.Defines.Process.Step.VisionProcess;
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
        private readonly ICamera? _camera;
        private readonly IVisionFlow? _flow;
        private ECylinder CurrentCylinder = ECylinder.PickHead1UpDown;
        private readonly Stopwatch _timer = new();
        private Task<GrabData>? _grab;
        private Task _cleanup = Task.CompletedTask;
        private int RetryCount;
        private CommonRecipe Recipe => _recipeList.CommonRecipe;
        #endregion
        #region Flags

        private IDInputDevice VirtualInput => _virtualIO.VisionProcessInput;
        private IDOutputDevice VirtualOutput => _virtualIO.VisionProcessOutput;
        private bool FlagTransferHead1VisionPositionReady
        {
            get { return VirtualInput[(int)EVisionProcessInput.TRANSFER_HEAD1_VISION_POSITION_READY]; }
        }
        private bool FlagTransferHead2VisionPositionReady
        {
            get { return VirtualInput[(int)EVisionProcessInput.TRANSFER_HEAD2_VISION_POSITION_READY]; }
        }
        private bool FlagTransferRequestVisionCheck
        {
            get { return VirtualInput[(int)EVisionProcessInput.TRANSFER_REQUEST_VISION_CHECK]; }
        }
        private bool FlagVisionRequestHead1Position
        {
            set { VirtualOutput[(int)EVisionProcessOutput.VISION_REQUEST_HEAD1_POSITION] = value; }
        }
        private bool FlagVisionRequestHead2Position
        {
            set { VirtualOutput[(int)EVisionProcessOutput.VISION_REQUEST_HEAD2_POSITION] = value; }
        }
        private bool FlagVisionRequestPlace
        {
            set { VirtualOutput[(int)EVisionProcessOutput.VISION_REQ_PLACE] = value; }
        }

        #endregion
        #region Constructors
        public VisionProcess(Devices devices, VirtualIO virtualIO, RecipeList recipeList,
            ICamera? camera = null, IVisionFlow? flow = null)
        {
            _devices = devices;
            _virtualIO = virtualIO;
            _recipeList = recipeList;
            _camera = camera;
            _flow = flow;
        }
        #endregion
        #region Override Methods
        public override bool ProcessOrigin()
        {
            switch ((EVisionProcessOriginStep)Step.OriginStep)
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
                    if (FlagTransferRequestVisionCheck)
                        Sequence = ESequence.Inspect;
                    else Wait(50);
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
                default:
                    Sequence = ESequence.Stop;
                    break;
            }
            return true;
        }
        #endregion
        #region Sequences
        private void Sequence_Ready()
        {
            switch ((EVisionProcessReadyStep)Step.RunStep)
            {
                case EVisionProcessReadyStep.Start:
                    Log.Debug("Vision Ready");
                    if (Recipe.SkipVisionCheck)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case EVisionProcessReadyStep.CheckCameraConnection:
                    Log.Debug("Check Camera Connection");
#if !SIMULATION
                    if (_camera == null || _flow == null)
                    {
                        RaiseAlarm(EAlarm.VisionCameraNotConfigured);
                        break;
                    }
                    if (!_camera.IsConnected)
                    {
                        RaiseAlarm(EAlarm.VisionCameraDisconnected);
                        break;
                    }
#endif
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
                    if (!FlagTransferRequestVisionCheck)
                    {
                        Wait(50);
                        break;
                    }
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
            switch ((EVisionProcessCheckStep)Step.RunStep)
            {
                case EVisionProcessCheckStep.Start:
                    Log.Debug("Vision Check Start");
                    RetryCount = 0;
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.IfSkipVision:
                    Log.Debug("Skip Vision");
                    if (Recipe.SkipVisionCheck)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.SetCurrentHead:
                    Log.Debug("Set Current Head");
                    RetryCount = 0;
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.Cylinder_Move_VisionPosition:
                    FlagVisionRequestHead1Position = true;
                    FlagVisionRequestHead2Position = true;
                    Log.Debug("Cylinder Move Vision Position");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.Cylinder_Move_VisionPosition_Wait:
#if !SIMULATION 
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.CylinderMoveVisionPositionTimeOut);
                        break;
                    }
#endif
                    Log.Debug("Cylinder Move Vision Position Done");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.ImageGrab:
                    Log.Debug("Image Grab");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.ImageGrab_Wait:
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.ImageGrabTimeOut);
                        break;
                    }
#endif
                    Log.Debug("Image Grab Done");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.VisionInspect:
                    Log.Debug("Vision Inspect");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.VisionInspect_Wait:
#if !SIMULATION
                    if(WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.VisionInspectTimeOut);
                        break;
                    }
#endif                 
                    Log.Debug("Vision Inspect Done");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.VisionRetryCheck:
                    Log.Debug("Vision Retry Check");
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
                    ((MappableOutputDevice<EVisionProcessOutput>)VirtualOutput).ClearOutputs();
                    Log.Debug("Vision Clear Flag");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.RequestTransferPlace:
                    FlagVisionRequestPlace = true;
                    _timer.Restart();
                    Log.Debug("Request Transfer Place");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.TransferRequestOff_Wait:
                    if (FlagTransferRequestVisionCheck)
                    {
                        if (_timer.ElapsedMilliseconds >= 10000)
                        {
                            FlagVisionRequestPlace = false;
                            RaiseAlarm(EAlarm.VisionInspectTimeOut);
                            break;
                        }
                        Wait(50);
                        break;
                    }
                    Log.Debug("Transfer Place Request Done");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.End:
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    Log.Debug("Vision End");
                    Sequence = ESequence.AutoRun;
                    break;
            }
        }
#endregion
    }
}
