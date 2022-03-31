using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using NLog.Web;

using Quartz;

namespace QuartzService
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

                                    services.AddOptions<QuartzOptions>().Bind(hostContext.Configuration.GetSection("Quartz"));

                                    // if you are using persistent job store, you might want to alter some options
                                    services.Configure<QuartzOptions>(options =>
                                    {
                                        options.Scheduling.IgnoreDuplicates = true; // default: false
                                        options.Scheduling.OverWriteExistingData = true; // default: true
                                    });

                                    services.AddTransient<HardCodedJob>();

                                    services.AddQuartz(q =>
                                    {

                                        q.UseMicrosoftDependencyInjectionJobFactory();
                                        q.UseSimpleTypeLoader();
                                        q.UseDefaultThreadPool(tp =>
                                        {
                                            tp.MaxConcurrency = 10;
                                        });

                                        var jobKey = new JobKey("job1", "group1");

                                        q.UseJobFactory<JobFactory>();

                                        q.AddJob<HardCodedJob>( job =>
                                        {
                                            job.WithIdentity(jobKey);
                                            job.UsingJobData("jobSays", "Hello World!");
                                        });

                                        q.AddTrigger(trigger =>
                                        {
                                            trigger.WithIdentity("trigger1", "group1");
                                            trigger.ForJob(jobKey);
                                            trigger.WithCronSchedule("10 0/1 * * * ?");
                                            //    trigger.WithSimpleSchedule(x => x
                                            //        .WithIntervalInSeconds(10)
                                            //        .RepeatForever());
                                            trigger.StartNow();
                                        });

                                        q.UsePersistentStore(s =>
                                        {
                                            s.UseProperties = true;
                                            s.RetryInterval = TimeSpan.FromSeconds(15);
                                            s.UseSqlServer(sqlServer =>
                                            {
                                                sqlServer.ConnectionString = connectionString;
                                            });
                                            s.UseJsonSerializer();
                                            s.UseClustering(c =>
                                            {
                                                c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                                                c.CheckinInterval = TimeSpan.FromSeconds(10);
                                            });
                                        });

                                    });

                                    services.AddQuartzHostedService(options =>
                                    {
                                        // when shutting down we want jobs to complete gracefully
                                        options.WaitForJobsToComplete = true;
                                    });

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
