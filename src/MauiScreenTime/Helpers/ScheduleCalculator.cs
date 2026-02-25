using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Helpers
{
    public static class ScheduleCalculator
    {
        public static TimeSpan CalculateDelayUntilNextRun(DateTime now, int hour)
        {
            var nextRun = now.Date.AddHours(hour);

            if (now >= nextRun)
                nextRun = nextRun.AddDays(1);

            return nextRun - now;
        }
    }
}
