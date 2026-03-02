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


        public DailyWorkerService(ICO2Service co2Service, IAppUsageDatabase appUsageDatabase, IUsageStatsService usageStatsService)//, IUserActivityLogDatabase userActivityLogDatabase)
        {

            _co2Service = co2Service;
            _appUsageDatabase = appUsageDatabase;
            _usageStatsService = usageStatsService;

            System.Diagnostics.Debug.WriteLine(_co2Service == null
            ? "DailyTaskService: _serviceA is NULL"
            : "DailyTaskService: _serviceA injected OK");
            System.Diagnostics.Debug.WriteLine(_appUsageDatabase == null
                ? "DailyTaskService: _serviceB is NULL"
                : "DailyTaskService: _serviceB injected OK");
        }
        public async Task StoreCO2TotalTodayAsync()
        {
            List<AppUsageModel>? appUsageList;  // declare outside so both blocks can access it
            try
            {   // object not set
                appUsageList = _usageStatsService.GetAppUsageAsync().GetAwaiter().GetResult();
                System.Diagnostics.Debug.WriteLine(appUsageList == null
                ? "DailyTaskService: data is NULL"
                : "DailyTaskService: data retrieved OK");
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DailyTaskService: GetDataAsync failed: {ex.Message}");
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
