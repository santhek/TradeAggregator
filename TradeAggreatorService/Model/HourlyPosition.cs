using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeAggreatorService.Model
{
    public class HourlyPosition
    {
        public HourlyPosition(DateTime startTime , DateTime endTime)
        {
            IntervalStart = startTime;
            IntervalEnd = endTime;
            StartHour = IntervalStart.Hour;
            TotalHours = (IntervalEnd - IntervalStart).Hours;
            Positions = new List<AggregatedPosition>();
        }
        public DateTime IntervalStart { get; set; }

        public DateTime IntervalEnd { get; set; }

        public int TotalHours { get; set; }

        public int StartHour { get; set; }
        public IList<AggregatedPosition> Positions { get; set; }
    }
}
