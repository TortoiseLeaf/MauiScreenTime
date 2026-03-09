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
                //this is superfluous and will throw an err later, if the conversion table db IS null what happens?
                //it stays null and we will try to access it anyway later -> null reference exception
                //we either check properly and have a plan b or we don't bother
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
                var conversionTableEntry = await _conversionTableDatabase.GetConversionTableEntryByPackageName(packageName);
                
                //if no conversion table entry is found should we use a default co2mins value? 
                if (conversionTableEntry == null) 
                {
                    System.Diagnostics.Debug.WriteLine($"No conversion entry found for: {appData.PackageName}");
                    return appData; //returns early, C02e is 0
                }
                                
                var CO2Mins = conversionTableEntry.CO2Mins;
                
                double appUsageMins = appData.UsageTimeMinutes;

                double CO2e = CO2Mins * appUsageMins;

                // write to the db or just do on the fly? performance/security 
                appData.CO2e = CO2e;
                appData.AppName = conversionTableEntry.AppName;
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

            }
            else
            {
                CO2Total = 0;
            }
           
            return CO2Total;
        }

        public async Task<double> CalculateCO2DifferenceAsync()
        {

            double yesterdayCO2Total;
            double dayBeforeYesterdayCO2Total;
            double differenceSaved = 0;


            var yesterdayTotalCO2Obj = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(DateTime.Now.AddDays(-1));
            yesterdayCO2Total = yesterdayTotalCO2Obj.CO2Total;

            var dayBeforeYesterdayTotalCO2Obj = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(DateTime.Now.AddDays(-2));
            dayBeforeYesterdayCO2Total = dayBeforeYesterdayTotalCO2Obj.CO2Total;

            if (yesterdayCO2Total > 0)
            {
                differenceSaved = dayBeforeYesterdayCO2Total - yesterdayCO2Total;

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
