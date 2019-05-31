using AM.Common.Validation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AM.AspNetCore.Hosting
{
    internal abstract class TimedHostedServiceBase : IHostedService, IDisposable
    {
        private Timer _timer;
        private Task _lastTask = Task.CompletedTask;
        private readonly IOptionsMonitor<TimeHostedServiceOptions> _periodMonitor;

        protected ILogger Logger { get; }

        protected TimedHostedServiceBase(IOptionsMonitor<TimeHostedServiceOptions> periodMonitor, ILogger logger)
        {
            Logger = logger;
            _periodMonitor = periodMonitor.Ensure(nameof(periodMonitor)).IsNotNull().Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(OnTimerElapsed, null, TimeSpan.Zero,
                _periodMonitor.CurrentValue.Period);

            return Task.CompletedTask;
        }

        private void OnTimerElapsed(object state)
        {
            if (!this._lastTask.IsCompleted)
                return;

            Logger.LogInformation("Timed Background Service is resuming to run periodic task.");
            _lastTask = this.DoWorkAsync(state);
        }

        protected abstract Task DoWorkAsync(object state);

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            await _lastTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
