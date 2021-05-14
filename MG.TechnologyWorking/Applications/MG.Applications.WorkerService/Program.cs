using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MG.Applications.WorkerService
{
    using MG.Logging;
    using MG.Logging.Providers;
    using MG.Shared.Configurations;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                }).ConfigureLogging((hostContext, builder) =>
                {
                    var loggers = hostContext.Configuration.GetSection("Loggers").Get<Loggers>()?.LoggerList.ToList();
                    foreach (var logger in loggers)
                    {
                        if (logger.Equals(nameof(SqlLogger)))
                        {
                            builder.AddProvider(new SqlLoggerProvider(hostContext.Configuration.GetConnectionString("TestDB")));
                        }
                        if (logger.Equals(nameof(FileLogger)))
                        {
                            builder.AddProvider(new FileLoggerProvider());
                        }
                    }
                }).UseWindowsService();
    }
}