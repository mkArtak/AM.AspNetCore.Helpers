# AM.AspNetCore.Helpers

As of now the code in this repo contains helper classes for creating generic host based workers, which will run periodically.

The following code is what you'll need to write to get the worker working:

#### Program.cs
```csharp
public satic async Task Mainusing Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace TimedWorker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .ConfigureDefaults()
                .ConfigureServices((hostContext, sc) =>
                {
                    sc.Configure<ServiceOptions>(hostContext.Configuration.GetSection("ServiceOptions"));
                    sc.AddHostedService<PeriodicHostedService>();
                }).Build();

            await hostBuilder.RunAsync();
        }
    }
}
```

### PeriodicHostedService.cs : This is the service class, which will host your custom logic
``` csharp
using AM.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace TimedWorker
{
    internal class PeriodicHostedService : TimedHostedServiceBase
    {
        public PeriodicHostedService(IOptionsMonitor<ServiceOptions> optionsMonitor, ILogger<PeriodicHostedService> logger) : base(optionsMonitor, logger)
        {
        }

        protected override Task DoWorkAsync(object state)
        {
            Logger.LogInformation("Periodic job done.");
            // TODO: Add your custom code here to run periodically
            return Task.CompletedTask;
        }
    }
}
```


The full sample can be found in the samples folder [here](https://github.com/mkArtak/AM.AspNetCore.Helpers/tree/master/samples/TimedWorker)

The package for AM.AspNetCore.Hosting project is available on [NuGet](https://www.nuget.org/packages/AM.AspNetCore.Hosting/).
