using EQX.Core.InOut;
using EQX.InOut.Virtual;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EQX.TemplateProject.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.TemplateProject.Extensions
{
    public static class AddVirtualIOExtension 
    {
        public static IHostBuilder AddProcessIO(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddKeyedSingleton<IDInputDevice, MappableInputDevice<ETransferProcessInput>>("TransferProcessInput");
                services.AddKeyedSingleton<IDOutputDevice, MappableOutputDevice<ETransferProcessOutput>>("TransferProcessOutput");

                services.AddKeyedSingleton<IDInputDevice, MappableInputDevice<ETrayProcessInput>>("TrayProcessInput");
                services.AddKeyedSingleton<IDOutputDevice, MappableOutputDevice<ETrayProcessOutput>>("TrayProcessOutput");
                services.AddKeyedSingleton<IDInputDevice, MappableInputDevice<EVisionProcessInput>>("VisionProcessInput");
                services.AddKeyedSingleton<IDOutputDevice, MappableOutputDevice<EVisionProcessOutput>>("VisionProcessOutput");

                services.AddSingleton<VirtualIO>();

            });

            return hostBuilder;
        }
    }
}
