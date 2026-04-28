using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidX.Work;

namespace MauiScreenTime.Platforms.Android.Workers
{
    public static class DailyWorkerScheduler
    {
        private const string WorkTag = "daily_13_00_task";

        public static void Schedule()
        {
            System.Diagnostics.Debug.WriteLine($"DailyWorkerScheduler fires");

            var context = global::Android.App.Application.Context;

            // Calculate initial delay until next 13:00
            var delay = GetDelayUntilNextTarget(targetHour: 13, targetMinute: 0);

            var workRequest = new OneTimeWorkRequest.Builder(typeof(DailyWorker))
                .SetInitialDelay(delay, Java.Util.Concurrent.TimeUnit.Milliseconds)
                .AddTag(WorkTag)
                .Build();

            WorkManager.GetInstance(context)
                .EnqueueUniqueWork(
                    WorkTag,
                    ExistingWorkPolicy.Replace,
                    (OneTimeWorkRequest)workRequest);

            System.Diagnostics.Debug.WriteLine($"Daily task scheduled. Fires in: {delay / 1000 / 60} minutes");
        }

        /// <summary>
        /// Called from DoWork() after task completes — schedules the next day's run.
        /// </summary>
        public static void ScheduleNext()
        {
            var context = global::Android.App.Application.Context;

            // Always 24 hours until the same time tomorrow
            var workRequest = new OneTimeWorkRequest.Builder(typeof(DailyWorker))
                .SetInitialDelay(24, Java.Util.Concurrent.TimeUnit.Hours)
                .AddTag(WorkTag)
                .Build();

            WorkManager.GetInstance(context)
                .EnqueueUniqueWork(
                    WorkTag,
                    ExistingWorkPolicy.Replace,
                    (OneTimeWorkRequest)workRequest);
        }

        private static long GetDelayUntilNextTarget(int targetHour, int targetMinute)
        {
            var now = DateTime.Now;
            var target = DateTime.Today.AddHours(13).AddMinutes(07);

            // If 13:00 has already passed today, schedule for tomorrow
            if (now >= target)
                target = target.AddDays(1);

            return (long)(target - now).TotalMilliseconds;
        }
    }
}