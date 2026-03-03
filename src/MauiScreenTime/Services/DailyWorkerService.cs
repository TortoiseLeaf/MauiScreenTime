using MauiScreenTime.Data;
using MauiScreenTime.Data.Interfaces;
using MauiScreenTime.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace MauiScreenTime.Services
{
    internal class DailyWorkerService : IDailyWorkerService
    {
        private readonly ICO2Service _co2Service;
        private readonly IAppUsageDatabase _appUsageDatabase;
        //private readonly IUserActivityLogDatabase _userActivityLogDatabase;
        private readonly IUsageStatsService _usageStatsService;


        public DailyWorkerService(ICO2Service co2Service, IAppUsageDatabase appUsageDatabase, IUsageStatsService usageStatsService)
        {

            _co2Service = co2Service;
            _appUsageDatabase = appUsageDatabase;
            _usageStatsService = usageStatsService;

        }
        public async Task StoreCO2TotalTodayAsync()
        {
            List<AppUsageModel>? appUsageList;  
            try
            {   
                appUsageList = _usageStatsService.GetAppUsageAsync().GetAwaiter().GetResult();
                
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DailyTaskService: GetAppUsageAsync failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"DailyTaskService: {ex.StackTrace}");

                return;
            }

            try
            {
                _co2Service.CalculateCO2TotalAsync(appUsageList).GetAwaiter().GetResult();
                System.Diagnostics.Debug.WriteLine("DailyTaskService: RunDailyTask saved to db completed");

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DailyTaskService: SaveToDatabase failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"DailyTaskService: {ex.StackTrace}");
            }
        }
    }
}
