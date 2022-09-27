using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeAggreatorService.Interface;
using TradeAggreatorService.Services;

namespace TradeAggreatorService
{
    internal class AggregationJob : IJob
    {
        private readonly ILogger<AggregationJob> _logger;
        private readonly IAggregator _aggregatorService;
        private readonly int _maxRetryCount = 3;
        public AggregationJob(ILogger<AggregationJob> logger,IAggregator aggregatorService, IConfiguration configuration)
        {
            _logger = logger;
            _aggregatorService = aggregatorService;
            if (configuration["App:MaxRetryCount"] != null)
                int.TryParse(configuration["App:MaxRetryCount"], out _maxRetryCount);

        }  

        Task IJob.Execute(IJobExecutionContext context)
        {
            var schedule = LondonTimeService.GetLondonTime();
            _logger.LogInformation("Service running at : {scheduleTime}", schedule);
            return DoJob(schedule,0);
        }

        private async Task DoJob(DateTime schedule, int retryCount = 0)
        {
            try
            {
                await _aggregatorService.ExportReportAsync(schedule);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Job at schedule {schedule} failed. This was the {retryCount + 1} attempt. Will retry a total of {_maxRetryCount} times.", ex);
                if (retryCount < _maxRetryCount)
                    await DoJob(schedule, retryCount++);
            }
        }
    }
    
}
