using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiScreenTime.Data;
using MauiScreenTime.Data.Interfaces;
using MauiScreenTime.Services.Interfaces;

namespace MauiScreenTime.Services
{
    public class CO2Service : ICO2Service
    {
        private readonly IConversionTableDatabase _conversionTableDatabase;
        private readonly IAppUsageDatabase _appUsageDatabase;
        private readonly UserActivityLogDatabase _userActivityLogDatabase;


        public CO2Service(IConversionTableDatabase conversionTableDatabase, IAppUsageDatabase appUsageDatabase, UserActivityLogDatabase userActivityLogDatabase)
        {
            // how much is superfluous, is it worth having the null check? else retry the connection? how to prevent recursion?
            if (conversionTableDatabase != null)
            {
                _conversionTableDatabase = conversionTableDatabase;
            }

            _appUsageDatabase = appUsageDatabase;
            _userActivityLogDatabase = userActivityLogDatabase;
        }
        public async Task<AppUsageModel> CalculateCO2eAsync(AppUsageModel appData)
        {

            try
            {
                var packageName = appData.PackageName;

                // trycatch
                var CO2Mins = await _conversionTableDatabase.GetMatchingCO2Mins(packageName);


                double appUsageMins = appData.UsageTimeMinutes;

                double CO2e = CO2Mins * appUsageMins;


                // write to the db or just do on the fly? performance/security 
                appData.CO2e = CO2e;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calculating CO2e in CO2Service : {ex}");
            }

            return appData;
        }

        public async Task<double> CalculateCO2TotalAsync(List<AppUsageModel> appUsageList)
        {
            double CO2Total = 0;

            if (appUsageList != null)
            {
                foreach (var appUsage in appUsageList)
                {
                    var newData = await CalculateCO2eAsync(appUsage);

                    CO2Total += newData.CO2e;
                }
                await _userActivityLogDatabase.AddActivityLog(CO2Total, 1);
                Console.WriteLine("added total to activity log successfully!");
            }
            else
            {
                CO2Total = 0; //error message explaining appUsageList is empty.
            }
            var x = await _userActivityLogDatabase.GetActivityByDate(DateTime.Now);

            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(x));
            Console.WriteLine("here now");
                return x.CO2Total; // return this to frontend. Save it to ActivityLog in the WorkManager service
        }
    }
}
