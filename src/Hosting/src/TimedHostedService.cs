using AM.Common.Validation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AM.AspNetCore.Hosting
{
    public abstract class TimedHostedServiceBase : IHostedService, IDisposable
    {
        private Timer _timer;
        private Task _lastTask = Task.CompletedTask;
        private readonly IOptionsMonitor<TimeHostedServiceOptions> _periodMonitor;

        /// <summary>
        /// Gets the configured logger instance.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Instantiates the object being created using the passed in option and logger.
        /// </summary>
        /// <param name="periodMonitor">The configuration options for the periodic execution</param>
        /// <param name="logger">The logger instance.</param>
        protected TimedHostedServiceBase(IOptionsMonitor<TimeHostedServiceOptions> periodMonitor, ILogger logger)
        {
            Logger = logger;
            _periodMonitor = periodMonitor.Ensure(nameof(periodMonitor)).IsNotNull().Value;
        }

        /// <summary>
        /// Starts the timed hosted service. This method is being called by the host.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to be used for interrupting execution</param>
        /// <returns>An asynchronously executing task tracking the `Start` request for this service instance.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(OnTimerElapsed, null, TimeSpan.Zero,
                _periodMonitor.CurrentValue.Period);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops the timed hosted service. This method is being called by the host.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to be used for interrupting execution</param>
        /// <returns>An asynchronously executing task tracking the `Stop` request for this service instance.</returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            await _lastTask;
        }

        /// <summary>
        /// Disposes the service instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Implementing classes should override this method if they need to dispose of other disposable resources.
        /// </summary>
        /// <param name="disposing">A boolean value indicating whether the instance is being disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer?.Dispose();
            }
        }

        /// <summary>
        /// Child classes must override this method to provide custom logic to be run when the service being invoked.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected abstract Task DoWorkAsync(object state);

        private void OnTimerElapsed(object state)
        {
            if (!this._lastTask.IsCompleted)
                return;

            Logger.LogInformation("Timed Background Service is resuming to run periodic task.");
            _lastTask = this.DoWorkAsync(state);
        }
    }
}
