using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using AndroidX.Work;
using MauiScreenTime.Services.Interfaces;
using Android.Util;

namespace MauiScreenTime.Platforms.Android
{
    public class DailyWorker : Worker
    {
       
        private readonly IDailyWorkerService _dailyWorkerService;

        public DailyWorker(Context context, WorkerParameters parameters)
        : base(context, parameters)
        {
            _dailyWorkerService = null;
        }

        public DailyWorker(Context context, WorkerParameters parameters, IDailyWorkerService dailyWorkerService)
            : base(context, parameters)
        {
            
            _dailyWorkerService = dailyWorkerService;
        }

        public override Result DoWork()
        {
            Log.Debug("DailyWorker", "here successfully fired the daily worker");
            try
            {
                

                _dailyWorkerService.StoreCO2TotalTodayAsync().GetAwaiter().GetResult();

                Log.Debug("DailyWorker", "here successfully fired the 23h total congrats");
                System.Diagnostics.Debug.WriteLine("here Successfully fired the 23h co2 Total congratulations");
                return Result.InvokeSuccess();
            }
            catch (Exception ex)
            {
                Log.Debug("DailyWorker", "Error, failed to fire the 23h co2Total");
                Log.Debug("DailyWorker", ex.Message);
                Log.Debug("DailyWorker", ex.StackTrace);

                return Result.InvokeFailure();
            }
        }
    }
}
