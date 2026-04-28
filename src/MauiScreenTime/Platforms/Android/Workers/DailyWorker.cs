using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using AndroidX.Work;
using global::Android.Content;

namespace MauiScreenTime.Platforms.Android.Workers
{
    public class DailyWorker : Worker
    {
        public DailyWorker(Context context, WorkerParameters workerParams)
            : base(context, workerParams) { }

        public override Result DoWork()
        {
            try
            {
                var now = DateTime.Now;
                if (now.Hour == 13 && now.Minute < 15) // refactor
                {
                    RunDailyTask();
                }

                // Schedule tomorrow's run before exiting
                DailyWorkerScheduler.ScheduleNext();

                return Result.InvokeSuccess();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DailyWorker failed: {ex.Message}");
                // Retry after 15 minutes on failure
                return Result.InvokeRetry();
            }
        }

        private void RunDailyTask()
        {
            // Your daily logic here
            System.Diagnostics.Debug.WriteLine("Daily task executed at 13:00!");
        }
    }
}