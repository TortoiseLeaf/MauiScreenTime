using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using AndroidX.Work;
using MauiScreenTime.Services.Interfaces;

namespace MauiScreenTime.Platforms.Android
{
    public class DailyWorker : Worker
    {
        private readonly ICO2Service _co2Service;
        private readonly IUsageStatsService _usageStatsService;


        public DailyWorker(Context context, WorkerParameters parameters, ICO2Service co2Service, IUsageStatsService usageStatsService)
            : base(context, parameters)
        {
            _co2Service = co2Service;
            _usageStatsService = usageStatsService;
        }

        public override Result DoWork()
        {
            try
            {
                var appUsageList = _usageStatsService.GetAppUsageAsync().GetAwaiter().GetResult();
                _co2Service.CalculateCO2TotalAsync(appUsageList);
                System.Diagnostics.Debug.WriteLine("here Successfully fired the 23h co2 Total");
                return Result.InvokeSuccess();
            }
            catch (Exception ex)
            {
                return Result.InvokeFailure();
            }
        }
    }
}
