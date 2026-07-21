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

        public CO2Service(IConversionTableDatabase conversionTableDatabase)
        {
            // how much is superfluous, is it worth having the null check? else retry the connection? how to prevent recursion?
                //this is superfluous and will throw an err later, if the conversion table db IS null what happens?
                //it stays null and we will try to access it anyway later -> null reference exception
                //we either check properly and have a plan b or we don't bother
            if (conversionTableDatabase != null)
            {
                _conversionTableDatabase = conversionTableDatabase;
            }           
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
                
                double appUsageMins = (double)appData.UsageTimeMinutes;

                double CO2e = CO2Mins * appUsageMins;

                // CAP THE CO2e at 300
                // write to the db or just do on the fly? performance/security 
                appData.CO2e = Math.Min(CO2e, 300); ;
                appData.AppName = conversionTableEntry.AppName;
                appData.Date = DateTime.UtcNow;
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
                    var appWithCO2e = await CalculateCO2eAsync(appUsage);

                    // cap total per app at 400g
                    CO2Total += Math.Min((double)appWithCO2e.CO2e, 300);
                    //CO2Total += newData.CO2e;
                }

            }
            else
            {
                CO2Total = 0;
            }
           
            return CO2Total;
        }   
    }
}
