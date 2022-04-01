using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using NLog.Web;

using Hangfire;
using Hangfire.SqlServer;

using HangfireService.HostedServices;

namespace HangfireService
{
    class Program
    {

        private static async Task Main(string[] args)
        {

            var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();

            try
            {
                var builder = Host.CreateDefaultBuilder()
                                .ConfigureHostConfiguration(config =>
                                {

                                    config.AddCommandLine(args);

                                })
                                .ConfigureLogging((hostingContext, logging) =>
                                {

                                    var sect = hostingContext.Configuration.GetSection("Logging");
                                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                                    logging.ClearProviders();
                                    logging.AddConsole();

                                })
                                .ConfigureServices((hostContext, services) =>
                                {

                                    var connectionString = hostContext.Configuration.GetConnectionString("Connection");

                                    services.AddHangfire(configuration => configuration
                                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                                        .UseSimpleAssemblyNameTypeSerializer()
                                        .UseRecommendedSerializerSettings()
                                        .UseSqlServerStorage(connectionString,
                                            new SqlServerStorageOptions
                                            {
                                                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                                                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                                                QueuePollInterval = TimeSpan.Zero,
                                                UseRecommendedIsolationLevel = true,
                                                DisableGlobalLocks = true
                                            }));

                                    services.AddHangfireServer();

                                    services.AddHostedService<ScheduleService>();

                                })
                                .UseNLog();

                await builder.RunConsoleAsync();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

    }

}
