using System;

using Microsoft.Extensions.Logging;

using Quartz;
using Quartz.Spi;

namespace QuartzService
{

    public class JobFactory : IJobFactory
    {

        private readonly ILogger<JobFactory> _logger;
        private readonly IServiceProvider _serviceProvider;

        public JobFactory(ILogger<JobFactory> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                IJobDetail jobDetail = bundle.JobDetail;
                Type jobType = jobDetail.JobType;
                _logger.LogDebug($"Producing instance of Job '{jobDetail.Key}', class={jobType.FullName}");

                return _serviceProvider.GetService(jobType) as IJob;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SchedulerException($"Problem instantiating class '{bundle.JobDetail.JobType.FullName}'", ex);
            }

        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }

    }

}
