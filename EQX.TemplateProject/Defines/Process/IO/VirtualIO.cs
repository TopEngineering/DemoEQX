using EQX.Core.InOut;
using EQX.InOut;
using EQX.InOut.Virtual;
using Microsoft.Extensions.DependencyInjection;

namespace EQX.TemplateProject.Defines
{
    public class VirtualIO
    {
        #region private
        private readonly IDInputDevice<ETrayProcessInput> _trayProcessInput;
        private readonly IDOutputDevice<ETrayProcessOutput> _trayProcessOutput;
        private readonly IDInputDevice<ETransferProcessInput> _transferProcessInput;
        private readonly IDOutputDevice<ETransferProcessOutput> _transferProcessOutput;
        private readonly IDInputDevice<EVisionProcessInput> _visionProcessInput;
        private readonly IDOutputDevice<EVisionProcessOutput> _visionProcessOutput;
        #endregion
        public VirtualIO([FromKeyedServices("TransferProcessInput")] IDInputDevice transferProcessInput,
                         [FromKeyedServices("TransferProcessOutput")] IDOutputDevice transferProcessOutput,
                         [FromKeyedServices("TrayProcessInput")] IDInputDevice trayProcessInput,
                         [FromKeyedServices("TrayProcessOutput")] IDOutputDevice trayProcessOutput,
                         [FromKeyedServices("VisionProcessInput")] IDInputDevice visionProcessInput,
                         [FromKeyedServices("VisionProcessOutput")] IDOutputDevice visionProcessOutput)
        {
            _trayProcessInput = (IDInputDevice<ETrayProcessInput>) trayProcessInput;
            _trayProcessOutput = (IDOutputDevice<ETrayProcessOutput>)trayProcessOutput;
            _transferProcessInput = (IDInputDevice<ETransferProcessInput>)transferProcessInput;
            _transferProcessOutput = (IDOutputDevice<ETransferProcessOutput>)transferProcessOutput;
            _visionProcessInput = (IDInputDevice<EVisionProcessInput>)visionProcessInput;
            _visionProcessOutput = (IDOutputDevice<EVisionProcessOutput>)visionProcessOutput;
        }

        public IDInputDevice TransferProcessInput { get; }
        public IDOutputDevice TransferProcessOutput { get; }

        public void Initialize()
        {
            _trayProcessInput.Initialize();
            _trayProcessOutput.Initialize();
            _transferProcessInput.Initialize();
            _transferProcessOutput.Initialize();
            _visionProcessInput.Initialize();
            _visionProcessOutput.Initialize();

        }

        public void Mappings()
        {
            #region TrayProcess to TransferProcess
            _trayProcessInput[ETrayProcessInput.TRANSFER_REQUEST_PICK_CELL].MapTo(_transferProcessOutput[ETransferProcessOutput.TRANSFER_REQUEST_PICK_CELL]);
            _trayProcessInput[ETrayProcessInput.TRANSFER_REQUEST_PLACE_CELL].MapTo(_transferProcessOutput[ETransferProcessOutput.TRANSFER_REQUEST_PLACE_CELL]);
            _trayProcessInput[ETrayProcessInput.TRANSFER_PICK_DONE].MapTo(_transferProcessOutput[ETransferProcessOutput.TRANSFER_PICK_DONE]);
            _trayProcessInput[ETrayProcessInput.TRANSFER_PLACE_DONE].MapTo(_transferProcessOutput[ETransferProcessOutput.TRANSFER_PLACE_DONE]);
            #endregion
            #region TransferProcess to TrayProcess
            _transferProcessInput[ETransferProcessInput.TRAY_PICK_CELL_READY].MapTo(_trayProcessOutput[ETrayProcessOutput.TRAY_PICK_CELL_READY]);
            _transferProcessInput[ETransferProcessInput.TRAY_PLACE_CELL_READY].MapTo(_trayProcessOutput[ETrayProcessOutput.TRAY_PLACE_CELL_READY]);
            _transferProcessInput[ETransferProcessInput.TRAY_PICK_DONE].MapTo(_trayProcessOutput[ETrayProcessOutput.TRANSFER_PICK_DONE]);
            _transferProcessInput[ETransferProcessInput.TRAY_PLACE_DONE].MapTo(_trayProcessOutput[ETrayProcessOutput.TRANSFER_PLACE_DONE]);
            #endregion
            #region TransferProcess to VisionProcess
            _transferProcessInput[ETransferProcessInput.VISION_CHECK_DONE].MapTo(_visionProcessOutput[EVisionProcessOutput.VISION_CHECK_DONE]);
            _visionProcessInput[EVisionProcessInput.TRANSFER_REQUEST_VISION_CHECK].MapTo(_transferProcessOutput[ETransferProcessOutput.TRANSFER_REQUEST_VISION_CHECK]);
            #endregion
        }
    }
}
