using AM.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace TimedWorker
{
    internal class PeriodicHostedService : TimedHostedServiceBase
    {
        private readonly IOptionsMonitor<ServiceOptions> _optionsMonitor;

        public PeriodicHostedService(IOptionsMonitor<ServiceOptions> optionsMonitor, ILogger<PeriodicHostedService> logger) : base(optionsMonitor, logger)
        {
            _optionsMonitor = optionsMonitor;
        }

        protected override Task DoWorkAsync(object state)
        {
            Logger.LogInformation("Periodic job done.");

            return Task.CompletedTask;
        }
    }
}
