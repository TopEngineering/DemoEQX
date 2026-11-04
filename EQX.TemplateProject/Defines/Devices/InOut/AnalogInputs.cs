using EQX.Core.InOut;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.TemplateProject.Defines.Devices
{
    public class AnalogInputs
    {
        private readonly IAInputDevice _aInputDevice;

        public AnalogInputs([FromKeyedServices("AnalogInputDevice#1")] IAInputDevice aInputDevice)
        {
            _aInputDevice = aInputDevice;
            Initialize();
        }

        public IAInput PlasmaVoltage => _aInputDevice.AnalogInputs.First(a => a.Id == (int)EAnalogInput.PLASMA_VOLTAGE);
        public IAInput PlasmaPower => _aInputDevice.AnalogInputs.First(a => a.Id == (int)EAnalogInput.PLASMA_POWER);

        public bool Initialize()
        {
            return _aInputDevice.Initialize();
        }

        public bool Connect()
        {
            return _aInputDevice.Connect();
        }

        public bool Disconnect()
        {
            return _aInputDevice.Disconnect();
        }
    }
}
