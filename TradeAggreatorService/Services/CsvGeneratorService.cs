using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeAggreatorService.Interface;
using TradeAggreatorService.Model;

namespace TradeAggreatorService.Services
{
    public class CsvGeneratorService : IExportReport
    {
        private readonly IConfiguration _config;

        public CsvGeneratorService(IConfiguration configuration)
        {
            _config = configuration;

        }

        public void GenerateReportAsync(HourlyPosition hourlyPosition)
        {
            var filePath = Path.Combine(GetFilePath(), GetFileName(hourlyPosition.IntervalEnd));
            StringBuilder resultCsv = new StringBuilder("HHMM,Volume");
            foreach (var pos in hourlyPosition.Positions)
            {
                resultCsv.Append($"{Environment.NewLine}{GetLocalTime(pos.Hour, hourlyPosition.IntervalEnd)} , {pos.Position}");
            }

            File.WriteAllText($"{filePath}", resultCsv.ToString());
        }

        public string GetLocalTime(int period, DateTime scheduleDate)
        {
            return scheduleDate.Date.AddHours(period - 1).ToString("HH:mm");

        }
        public string GetFileName(DateTime schedule)
        {
            return $"PowerPosition_{schedule.ToString("yyyyMMdd")}_{schedule.ToString("HHmm")}.csv";
        }

        public string GetFilePath()
        {
            if (string.IsNullOrEmpty(_config["App:CsvPath"]) || !Directory.Exists(_config["App:CsvPath"]))
                throw new ArgumentException("config param App:CsvPath is missing or the direcory specified does not exist");
            return _config["App:CsvPath"];
        }
    }
}
