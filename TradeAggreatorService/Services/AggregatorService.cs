using Microsoft.Extensions.Logging;
using Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeAggreatorService.Interface;
using TradeAggreatorService.Model;
using System.Linq;
using Quartz;

namespace TradeAggreatorService.Services
{
    public class AggregatorService : IAggregator
    {
        private readonly IPowerService _powerService;
        private readonly ILogger<AggregatorService> _logger;
        private readonly IExportReport _exporterService;
        public AggregatorService(IPowerService powerservice, ILogger<AggregatorService> logger, IExportReport exporterService)
        {
            _powerService = powerservice;
            _logger = logger;
            _exporterService = exporterService;
        }

        public async Task ExportReportAsync(DateTime schedule)
        {
            var hourlyPostions = await GetAggregatedPostionsAsync(schedule);
            _exporterService.GenerateReportAsync(hourlyPostions);
        }

        public async Task<HourlyPosition> GetAggregatedPostionsAsync(DateTime schedule)
        {
            var trades = await GetTradesAsync(schedule);
            var window = GetTradeTimeWindow(schedule);
            HourlyPosition result = new HourlyPosition(window.Item1, window.Item2);
            
            for (var hour = 1; hour <= result.TotalHours + 1; hour++)
            {
                int period = hour;
                period = (period == 1) ? 0 : hour - 1;
                     
                var subSet = trades.Where(tt => tt.Date >= result.IntervalStart &&
                tt.Date <= result.IntervalEnd);
                var position = subSet.SelectMany(p => p.Periods.Where(pp => pp.Period == hour)).Sum(p => p.Volume);
                AggregatedPosition resultItem = new AggregatedPosition(period, position);
                result.Positions.Add(resultItem);
                
            }
            return result;
        }
       

        public async Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime schedule)
        {
            IEnumerable<PowerTrade> trades;
            try
            {
                trades = await _powerService.GetTradesAsync(schedule);                
                
            }
            catch (PowerServiceException px)
            {
                _logger.LogError("Error in retrieving trades from Power Service", px);
                throw;
            }
            return trades;
        }

        public Tuple<DateTime,DateTime> GetTradeTimeWindow(DateTime schedule)
        {
            var startTime  = schedule.Date.AddHours(-1);
            return new Tuple<DateTime, DateTime>(startTime,schedule);
        }
    }
}
