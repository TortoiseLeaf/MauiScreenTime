using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiScreenTime.Data;
using MauiScreenTime.Data.Interfaces;
using MauiScreenTime.Services.Interfaces;
#if ANDROID
using Android.Util;
#endif

namespace MauiScreenTime.Services
{
    public class CO2Service : ICO2Service
    {
        private readonly IConversionTableDatabase _conversionTableDatabase;
        private readonly IAppUsageDatabase _appUsageDatabase;
        private readonly IUserActivityLogDatabase _userActivityLogDatabase;


        public CO2Service(IConversionTableDatabase conversionTableDatabase, IAppUsageDatabase appUsageDatabase, IUserActivityLogDatabase userActivityLogDatabase)
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

                // call this once a day or 
                //await _userActivityLogDatabase.AddActivityLog(CO2Total, 0, 0);

            }
            else
            {
                CO2Total = 0; //error message explaining appUsageList is empty.
            }
           
            return CO2Total;
        }

        public async Task<double> CalculateCO2DifferenceAsync()
        {

            double todayCO2Total;
            double yesterdayCO2Total;
            double differenceSaved = 0;


            var todayTotalCO2Obj = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(DateTime.Now);
            todayCO2Total = todayTotalCO2Obj.CO2Total;

            var yesterdayTotalCO2Obj = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(DateTime.Now.AddDays(-1));
            yesterdayCO2Total = yesterdayTotalCO2Obj.CO2Total;

            //// debug to show yesterday and todays entries with the highest CO2Total
            //var today = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(DateTime.Now);
            //var yesterday = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(DateTime.Now.AddDays(-1));

            //Console.WriteLine("here calculate diff");
            //Console.WriteLine(todayCO2Total);
            //Console.WriteLine(yesterdayCO2Total);
            //Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(today));
            //Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(yesterday));


            if (yesterdayCO2Total > 0)
            {
                differenceSaved = yesterdayCO2Total - todayCO2Total;

                if (differenceSaved > 0)
                {
                    Console.WriteLine("This is the difference saved: " + differenceSaved);

                    try
                    {
                        await _userActivityLogDatabase.AddActivityLog(0, differenceSaved, 0);
                        Console.WriteLine("diff saved successfully");

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error adding diff: ", ex.Message);

                    }

#if ANDROID
                    Log.Debug("CO2Service", "here successfully CO2 difference saved to log");
#endif

                }
            }

            return differenceSaved;
        }
    }
    }
