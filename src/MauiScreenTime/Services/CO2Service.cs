using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiScreenTime.Data;

namespace MauiScreenTime.Services
{
    internal class CO2Service : ICO2Service
    {
            private readonly ConversionTableDatabase _conversionTableDatabase;
            private readonly AppUsageDatabase _appUsageDatabase;


        public CO2Service(ConversionTableDatabase conversionTableDatabase, AppUsageDatabase appUsageDatabase)
        {
            // how much is superfluous, is it worth having the null check? else retry the connection? how to prevent recursion?
            if (conversionTableDatabase != null)
            {
                _conversionTableDatabase = conversionTableDatabase;
            }

            _appUsageDatabase = appUsageDatabase;
        }
        public async Task<AppUsageModel> CalculateCO2eAsync(AppUsageModel appData)
            {

            var packageName = appData.PackageName;

            var conversionTable = await _conversionTableDatabase.GetConversionTable();

            if (conversionTable != null)
            {

                var conversionObject = conversionTable.FirstOrDefault(x => x.PackageName == packageName);

                //Console.WriteLine(conversionObject.CO2Mins);
                //Console.WriteLine("here after conversion object");

                if (conversionObject != null)
                {
                    double CO2fromTable = conversionObject.CO2Mins;
                    double appUsageMins = appData.UsageTimeMinutes;

                    double CO2e = CO2fromTable * appUsageMins;
                    
                    //add it to the appUsageModel?

                    // does this write it to the db? ********************
                    // can just overwrite each previous entry for the app.
                    appData.CO2e = CO2e;


                }
            }
            Console.WriteLine(appData.CO2e);

            return appData;
        }
    }
    }
