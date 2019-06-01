using Microsoft.Extensions.DependencyInjection;
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
