using Android.Content;
using Android.Content;
using AndroidX.Work;
using AndroidX.Work;
using global::Android.Content;
using MauiScreenTime.Services;
using MauiScreenTime.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Platforms.Android.Workers
{
    public class DailyWorker : Worker
    {
        private readonly IDailyWorkerService _dailyWorkerService;

        public DailyWorker(Context context, WorkerParameters workerParams)
    : base(context, workerParams)
        {
            try
            {
                _dailyWorkerService = AndroidServiceLocator.GetService<IDailyWorkerService>();
                System.Diagnostics.Debug.WriteLine("DailyWorker: IMyService resolved successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DailyWorker: Failed to resolve IMyService: {ex.Message}");
            }
        }

        public DailyWorker(Context context, WorkerParameters workerParams, IDailyWorkerService dailyWorkerService)
        : base(context, workerParams)
        {
            _dailyWorkerService = dailyWorkerService;
        }


        public override Result DoWork()
        {
            System.Diagnostics.Debug.WriteLine("DailyWorker: DoWork started");

            if (_dailyWorkerService == null)
            {
                System.Diagnostics.Debug.WriteLine("DailyWorker: _dailyWorkerService is null, cannot proceed");
                return Result.InvokeFailure();
            }

            try
            {
                var now = DateTime.Now;
                System.Diagnostics.Debug.WriteLine($"DailyWorker: Running at {now:HH:mm}");

                //if (now.Hour == 13 && now.Minute < 15)
                //{
                    System.Diagnostics.Debug.WriteLine("DailyWorker: Calling dailyWorkerService...");
                    _dailyWorkerService.StoreCO2TotalTodayAsync();
                    System.Diagnostics.Debug.WriteLine("DailyWorker: dailyWorkerService completed");
                //}

                DailyWorkerScheduler.ScheduleNext();
                return Result.InvokeSuccess();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DailyWorker: Exception in DoWork: {ex.Message}");
                return Result.InvokeRetry();
            }
        }
    }
}