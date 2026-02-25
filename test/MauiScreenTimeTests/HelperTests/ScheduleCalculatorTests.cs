using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using MauiScreenTime.Helpers;

namespace MauiScreenTimeTests.HelperTests
{
    public class ScheduleCalculatorTests
    {
        [Fact]
        public void Before1pm_DelayIsWithinSameDay()
        {
            var now = DateTime.Today.AddHours(10); // 10:00 AM
            var delay = ScheduleCalculator.CalculateDelayUntilNextRun(now, 13);

            Assert.Equal(3, delay.TotalHours); // Should be 3 hours away
        }

        [Fact]
        public void After1pm_DelayIsNextDay()
        {
            var now = DateTime.Today.AddHours(14); // 2:00 PM
            var delay = ScheduleCalculator.CalculateDelayUntilNextRun(now, 13);

            Assert.Equal(23, delay.TotalHours); // Should be 23 hours away
        }

        [Fact]
        public void Exactly1pm_DelayIsNextDay()
        {
            var now = DateTime.Today.AddHours(13); // Exactly 1:00 PM
            var delay = ScheduleCalculator.CalculateDelayUntilNextRun(now, 13);

            Assert.Equal(24, delay.TotalHours); // Should schedule for tomorrow
        }

        [Fact]
        public void DelayIsNeverNegative()
        {
            var now = DateTime.Now;
            var delay = ScheduleCalculator.CalculateDelayUntilNextRun(now, 13);

            Assert.True(delay.TotalMinutes > 0);
        }
    }

}
