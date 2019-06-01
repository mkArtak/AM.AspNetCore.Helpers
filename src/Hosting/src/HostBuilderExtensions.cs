using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Encapsulates a set of extension methods for <see cref="HostBuilder"/> type. 
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Applies an oppiniated default logging and configuration settings to the host builder.
        /// </summary>
        /// <param name="hostBuilder">The host builder to configure.</param>
        /// <returns>The passed in instance.</returns>
        /// <remarks>
        /// The default configuration accepts settings through `appsettings[.environmentName].json` file(s).
        /// By default `Console` and `Debug` logging providers are configured.
        /// </remarks>
        public static HostBuilder ConfigureDefaults(this HostBuilder hostBuilder)
        {
            hostBuilder
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile("appsettings.json", optional: true);
                    configApp.AddJsonFile(
                        $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                        optional: true);
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                })
                .UseConsoleLifetime();

            return hostBuilder;
        }
    }
}
