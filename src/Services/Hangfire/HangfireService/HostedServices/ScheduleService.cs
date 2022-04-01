using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

using Hangfire;

using HangfireService.Jobs;

namespace HangfireService.HostedServices
{

    internal class ScheduleService : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            RecurringJob.AddOrUpdate<HardCodedJob>("job1", (job) => job.Execute(), "10 0/1 * * * ?");
            await Task.Delay(TimeSpan.Zero, stoppingToken);
        }

    }

}
