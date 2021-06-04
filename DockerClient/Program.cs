using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DockerClient
{
    public class Program
    {
        private static ILogger<Program> _logger;

        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            DockerManager.Init(host.Services);
            _logger = host.Services.GetRequiredService<ILogger<Program>>();
            var dockerMan = host.Services.GetRequiredService<DockerManager>();
            
            if (await dockerMan.CheckIfImageIsBuilt().ConfigureAwait(false))
            {
                _logger.LogInformation("Image is already built");
            }
            else
            {
                _logger.LogWarning("Image doesn't exist");
                _logger.LogInformation("Building image");
                await dockerMan.BuildImage().ConfigureAwait(false);
                _logger.LogInformation("Built image");
            }
            if (args.Any(a => a.Equals("-v", StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }
            await host.RunAsync();
        }
            
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                .ConfigureServices((ctx ,sc)=>
                {
                    sc.AddScoped<DockerManager>();
                    sc.AddHostedService<CommandExecutorService>();
                    sc.Configure<AppOptions>(ctx.Configuration.GetSection(AppOptions.SectionName));
                });
    }
}