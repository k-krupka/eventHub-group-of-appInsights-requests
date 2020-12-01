using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eventHub_group_of_appInsights_requests
{
    public class Program
    {
        static Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<ExampleHostedService>();

                    services.AddLogging(builder =>
                    {
                        builder.Services.AddApplicationInsightsTelemetryWorkerService(options =>
                        {
                            options.InstrumentationKey = Configuration.InstrumentationKey;
                            options.EnableAdaptiveSampling = false;

                            options.DeveloperMode = false;

                            options.EnableHeartbeat = false;
                            options.EnableEventCounterCollectionModule = false;
                            options.EnableDiagnosticsTelemetryModule = false;

                            options.EnableDependencyTrackingTelemetryModule = true;
                        });
                    });

                });

            return hostBuilder
                .UseConsoleLifetime()
                .Build()
                .RunAsync();
        }
    }
}
