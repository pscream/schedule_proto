using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Quartz;

namespace QuartzService
{

    public class HardCodedJob : IJob
    {

        private readonly ILogger<HardCodedJob> _logger;

        public HardCodedJob(ILogger<HardCodedJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogInformation($"Greetings from HardCodedJob at {DateTime.Now}");
                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw new JobExecutionException(); // Look at its documentation
            }
        }
    }

}
