using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeAggreatorService.Model;

namespace TradeAggreatorService.Interface
{
    public interface IExportReport
    {
        string GetFilePath();

        void GenerateReportAsync(HourlyPosition hourlyPosition);

        string GetFileName( DateTime schedule);
    }
}
