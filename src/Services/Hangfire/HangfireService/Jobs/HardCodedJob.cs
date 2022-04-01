using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace HangfireService.Jobs
{

    public class HardCodedJob
    {

        private readonly ILogger<HardCodedJob> _logger;

        public HardCodedJob(ILogger<HardCodedJob> logger)
        {
            _logger = logger;
        }

        public Task Execute()
        {
            _logger.LogInformation($"Greetings from HardCodedJob at {DateTime.Now}");
            return Task.CompletedTask;
        }

    }

}
