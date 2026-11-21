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
using EQX.TemplateProject.Defines.Vision;

namespace EQX.TemplateProject.Process
{
    public class VisionProcess : ProcessBase<ESequence>
    {
        #region private
        private readonly Devices _devices;
        private readonly VirtualIO _virtualIO;
        private readonly RecipeList _recipeList;
        private readonly CVisionAlignResults _visionAlignResults;
        VisionAlignResult result;

        private readonly ICamera? _camera;
        private readonly IVisionFlow? _flow;
        private ECylinder CurrentCylinder = ECylinder.PickHead1;
        private readonly Stopwatch _timer = new();
        private int RetryCount;
        private CommonRecipe Recipe => _recipeList.CommonRecipe;
        #endregion
        #region Flags

        private IDInputDevice VirtualInput => _virtualIO.VisionProcessInput;
        private IDOutputDevice VirtualOutput => _virtualIO.VisionProcessOutput;
        private bool FlagTransferRequestVisionInspect1
        {
            get { return VirtualInput[(int)EVisionProcessInput.TRANSFER_REQ_INSPECT1]; }
        }
        private bool FlagTransferRequestVisionInspect2
        {
            get { return VirtualInput[(int)EVisionProcessInput.TRANSFER_REQ_INSPECT2]; }
        }
        private bool FlagVisionInspect1Done
        {
            set { VirtualOutput[(int)EVisionProcessOutput.VISION_INSPECT1_DONE] = value; }
        }
        private bool FlagVisionInspect2Done
        {
            set { VirtualOutput[(int)EVisionProcessOutput.VISION_INSPECT2_DONE] = value; }
        }
        #endregion
        #region IsBool
        private bool IsCurrentHeadSkipped => CurrentCylinder switch
        {
            ECylinder.PickHead1 => Recipe.SkipCylinder1,
            ECylinder.PickHead2 => Recipe.SkipCylinder2,
            _ => false,
        };
        #endregion
        #region Constructors
        public VisionProcess(Devices devices, VirtualIO virtualIO, RecipeList recipeList,CVisionAlignResults visionAlignResults,
            ICamera? camera = null, IVisionFlow? flow = null)
        {
            _devices = devices;
            _virtualIO = virtualIO;
            _recipeList = recipeList;
            _visionAlignResults = visionAlignResults;
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
                    if (FlagTransferRequestVisionInspect1 || FlagTransferRequestVisionInspect2)
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
                    if (!FlagTransferRequestVisionInspect1 && !FlagTransferRequestVisionInspect2)
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
                    _visionAlignResults.Reset();
                    RetryCount = 0;
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.FlagTranserREQInspect:
                    if (!FlagTransferRequestVisionInspect1 && !FlagTransferRequestVisionInspect2)
                    {
                        Wait(50);
                        break;
                    }
                    Log.Debug("Transfer Request Inspect");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.SetCurrentHead:
                    CurrentCylinder = ECylinder.PickHead1;
                    _timer.Restart();
                    if (!Recipe.SkipCylinder1)
                    {
                        Log.Debug($"Under Vision {CurrentCylinder} Start");
                        Step.RunStep = (int)EVisionProcessCheckStep.ImageGrab;
                    }
                    else
                    {
                        Step.RunStep = (int)EVisionProcessCheckStep.UpdateCurrentHead;
                    }
                    break;
                case EVisionProcessCheckStep.ImageGrab:
                    if ((CurrentCylinder == ECylinder.PickHead1 && !FlagTransferRequestVisionInspect1) ||
                        (CurrentCylinder == ECylinder.PickHead2 && !FlagTransferRequestVisionInspect2))
                    {
                        if (_timer.Elapsed.TotalSeconds >= Recipe.MotionMoveTimeOut)
                        {
                            RaiseAlarm(EAlarm.CylinderMoveVisionPositionTimeOut);
                            break;
                        }
                        Wait(20);
                        break;
                    }
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
                    if (CurrentCylinder == ECylinder.PickHead1)
                    {
                        Step.RunStep = (int)EVisionProcessCheckStep.VisionInspect1;
                    }
                    else
                    {
                        Step.RunStep = (int)EVisionProcessCheckStep.VisionInspect2;
                    }
                    break;
                case EVisionProcessCheckStep.VisionInspect1:
                    if (Recipe.SkipCylinder1)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("Vision Inspect 1");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.VisionInspect1_Wait:
                    if (Recipe.SkipCylinder1)
                    {
                        Step.RunStep++;
                        break;
                    }
#if !SIMULATION
                    if(WaitTimeOutOccurred)
                    {
                        RaiseAlarm(EAlarm.VisionInspectTimeOut);
                        break;
                    }
#endif
                    _visionAlignResults.VisionAlignResult1.Judge = EVisionJudge.OK;
                    Log.Debug("Vision Inspect 1 Done");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.VisionInspect2:
                    if (CurrentCylinder == ECylinder.PickHead1)
                    {
                        Step.RunStep = (int)EVisionProcessCheckStep.VisionRetryCheck;
                        break;
                    }
                    if (Recipe.SkipCylinder2)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("Vision Inspect 2");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.VisionInspect2_Wait:
                    if (Recipe.SkipCylinder2)
                    {
                        Step.RunStep++;
                        break;
                    }
#if !SIMULATION
                    if (WaitTimeOutOccurred)
                    {
                        Log.Error("Vision Inspect 2 timeout");
                        RaiseAlarm(EAlarm.VisionInspectTimeOut);
                        break;
                    }
#endif
                    _visionAlignResults.VisionAlignResult2.Judge = EVisionJudge.OK;
                    Log.Debug("Vision Inspect 2 Done");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.VisionRetryCheck:
                {
                    if (IsCurrentHeadSkipped)
                    {
                        Step.RunStep++;
                        break;
                    }
                    if (CurrentCylinder == ECylinder.PickHead1)
                        result = _visionAlignResults.VisionAlignResult1;
                    else
                        result = _visionAlignResults.VisionAlignResult2;

                    if (result.Judge == EVisionJudge.OK)
                    {
                        Step.RunStep++;
                        break;
                    }

                    if (RetryCount < _recipeList.VisionRecipe.RetryCount)
                    {
                        RetryCount++;
                        Step.RunStep = (int)EVisionProcessCheckStep.ImageGrab;
                        break;
                    }
                    Log.Debug("Vision Retry Check");
                    Step.RunStep++;
                    break;
                }
                case EVisionProcessCheckStep.ResultUpdate:
                    if (CurrentCylinder == ECylinder.PickHead1)
                        FlagVisionInspect1Done = true;
                    else
                        FlagVisionInspect2Done = true;
                    Log.Debug("Result Update");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.UpdateCurrentHead:
                    if (CurrentCylinder == ECylinder.PickHead1)
                    {
                        if (Recipe.SkipCylinder2)
                        {
                            Step.RunStep = (int)EVisionProcessCheckStep.Vision_CheckDone;
                            break;
                        }
                        CurrentCylinder = ECylinder.PickHead2;
                        Log.Debug($"Under Vision {CurrentCylinder} Start");
                        RetryCount = 0;
                        _timer.Restart();
                        Step.RunStep = (int)EVisionProcessCheckStep.ImageGrab;
                    }
                    else
                    {
                        Step.RunStep = (int)EVisionProcessCheckStep.Vision_CheckDone;
                    }
                    break;
                case EVisionProcessCheckStep.Vision_CheckDone:
                    _timer.Restart();
                    Log.Debug("Vision Check Done");
                    Step.RunStep++;
                    break;
                case EVisionProcessCheckStep.Vision_ClearFlags:
                    if (FlagTransferRequestVisionInspect1 || FlagTransferRequestVisionInspect2)
                    {
                        if (_timer.ElapsedMilliseconds >= 10000)
                        {
                            RaiseAlarm(EAlarm.VisionInspectTimeOut);
                            break;
                        }
                        Wait(20);
                        break;
                    }
                    ((MappableOutputDevice<EVisionProcessOutput>)VirtualOutput).ClearOutputs();
                    FlagVisionInspect1Done = false;
                    FlagVisionInspect2Done = false;
                    Log.Debug("Vision Clear Flag");
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
