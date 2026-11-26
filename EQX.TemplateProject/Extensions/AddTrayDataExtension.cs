using EQX.TemplateProject.MVVM.Models;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace EQX.TemplateProject.Extensions
{
    public static class AddTrayDataExtension
    {
        public static IHostBuilder AddTrayData(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                services.TryAddSingleton<TrayData>();
            });
            return hostBuilder;
        }
    }
}
