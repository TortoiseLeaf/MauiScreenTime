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
        private readonly IUsageStatsService _usageStatsService;
        private readonly IUserActivityLogDatabase _userActivityLogDatabase;


        public DailyWorkerService(ICO2Service co2Service, IAppUsageDatabase appUsageDatabase, IUsageStatsService usageStatsService, IUserActivityLogDatabase userActivityLogDatabase)
        {

            _co2Service = co2Service;
            _appUsageDatabase = appUsageDatabase;
            _usageStatsService = usageStatsService;
            _userActivityLogDatabase = userActivityLogDatabase;
        }
        public async Task StoreCO2TotalTodayAsync()
        {
            List<AppUsageModel>? appUsageList;
            try
            {

                appUsageList = _usageStatsService.GetAppUsageAsync().GetAwaiter().GetResult();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DailyTaskService: GetAppUsageAsync failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"DailyTaskService: {ex.StackTrace}");

                return;
            }

            try
            {
                // USE THIS AS DEBUGGER TO CHECK THE METHOD IS CALLING WHEN SCHEDULED
                //_userActivityLogDatabase.AddActivityLog(0, 0, 0,1).GetAwaiter().GetResult();

                // get the co2Total, and save it to the db
                var co2TotalToday = _co2Service.CalculateCO2TotalAsync(appUsageList).GetAwaiter().GetResult();
                _userActivityLogDatabase.AddActivityLog(co2TotalToday, 0, 0,0).GetAwaiter().GetResult();
                System.Diagnostics.Debug.WriteLine("DailyTaskService: RunDailyTask saved to db completed");

                // calculate and store to db the Co2 difference between yesterday and today
                _co2Service.CalculateAndStoreCO2DifferenceAsync().GetAwaiter().GetResult();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DailyTaskService: SaveToDatabase failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"DailyTaskService: {ex.StackTrace}");
            }
        }
    }
}