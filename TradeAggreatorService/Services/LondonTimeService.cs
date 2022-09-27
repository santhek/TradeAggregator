using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeAggreatorService.Services
{
    internal sealed class LondonTimeService
    {
        private static TimeZoneInfo britishZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        private LondonTimeService()
        {            
        }

        internal static DateTime GetLondonTime()
        {
            return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, britishZone);
        }
    }
}
