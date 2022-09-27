using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeAggreatorService.Model;

namespace TradeAggreatorService.Interface
{
    public interface IAggregator
    {
        Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime schedule);
        Task ExportReportAsync(DateTime schedule);

        Task<HourlyPosition> GetAggregatedPostionsAsync(DateTime schedule);
        Tuple<DateTime, DateTime> GetTradeTimeWindow(DateTime schedule);


    }
}
