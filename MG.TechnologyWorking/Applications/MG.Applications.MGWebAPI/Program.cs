using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MG.Applications.MGWebAPI
{
    using MG.Logging;
    using MG.DAL.Mongo;
    using MG.DAL.Cassandra;
    using MG.DAL.Repositories;
    using MG.Logging.Providers;
    using MG.DAL.Mongo.Interface;
    using MG.Services.FileService;
    using MG.Services.HttpServices;
    using MG.Shared.Configurations;
    using MG.DAL.Cassandra.Interface;
    using MG.DAL.EntityFrameworkCore;
    using MG.Services.FileService.Interface;
    using MG.Services.HttpServices.Interface;
    using MG.DAL.Repositories.EFCoreRepository;
    using MG.Queue.RabbitMQ.Interfaces;
    using MG.Queue.RabbitMQ;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureServices((hostContext, services) =>
                {
                    #region DB

                    services.AddDbContext<MGContext>(c => c.UseSqlServer(connectionString: hostContext.Configuration.GetConnectionString("Test")));

                    #endregion

                    #region Repositories

                    services.AddSingleton(typeof(ICassandraRepository<>), typeof(CassandraRepository<>));
                    services.AddScoped(typeof(IRepository<>), typeof(MGRepository<>));
                    services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
                    services.AddScoped<IRabbitMQService, RabbitMQService>();

                    #endregion

                    #region Services

                    services.AddScoped<IFileService, FileService>();

                    #endregion

                    services.AddScoped<IHttpService, HttpService>();

                }).ConfigureLogging((hostContext, builder) =>
                {
                    var loggers = hostContext.Configuration.GetSection("Loggers").Get<Loggers>()?.LoggerList?.ToList();
                    foreach (var logger in loggers)
                    {
                        if (logger.Equals(nameof(SqlLogger)))
                        {
                            builder.AddProvider(new SqlLoggerProvider(hostContext.Configuration.GetConnectionString("Test")));
                        }
                        if (logger.Equals(nameof(FileLogger)))
                        {
                            builder.AddProvider(new FileLoggerProvider());
                        }
                    }
                });
    }
}