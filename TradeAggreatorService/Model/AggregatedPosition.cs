using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeAggreatorService.Model
{
    public class AggregatedPosition
    {
        public AggregatedPosition(int hour, double position)
        {
            Hour = hour;
            Position = position;
        }
    
        public int Hour { get; set; }
        public double Position { get; set; }
    }
}
