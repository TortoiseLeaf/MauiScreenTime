using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using AndroidX.Work;
using global::Android.Content;
using MauiScreenTime.Services.Interfaces;
using Android.Content;
using AndroidX.Work;

namespace MauiScreenTime.Platforms.Android.Workers
{
    public class DailyWorker : Worker
    {
        private readonly IDailyWorkerService _dailyWorkerService;

        public DailyWorker(Context context, WorkerParameters workerParams)
    : base(context, workerParams)
        {
            System.Diagnostics.Debug.WriteLine($"Daily first constructor used");

            _dailyWorkerService = IPlatformApplication.Current?.Services
                .GetRequiredService<IDailyWorkerService>();
        }

        public DailyWorker(
            Context context,
            WorkerParameters workerParams,
            IDailyWorkerService dailyWorkerService)      // <-- injected by MauiWorkerFactory
            : base(context, workerParams)
        {
            System.Diagnostics.Debug.WriteLine($"daily second constructor used");

            _dailyWorkerService = dailyWorkerService;
        }

        public override Result DoWork()
        {
            System.Diagnostics.Debug.WriteLine($"DailyWorker fired");

            try
            {
                var now = DateTime.Now;
                if (now.Hour == 13 && now.Minute < 15)
                {
                    _dailyWorkerService.StoreCO2TotalTodayAsync(); // use it normally
                    System.Diagnostics.Debug.WriteLine($"DailyWorker service method fired");

                }

                DailyWorkerScheduler.ScheduleNext();
                return Result.InvokeSuccess();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DailyWorker failed: {ex.Message}");
                return Result.InvokeRetry();
            }
        }
    }
}