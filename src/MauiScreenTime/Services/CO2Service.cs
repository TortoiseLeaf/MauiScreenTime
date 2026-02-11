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
            // how much is superfluous, is it worth having the null check? What message or alternative can the else do?
            if (conversionTableDatabase != null)
            {
                _conversionTableDatabase = conversionTableDatabase;
            }

            _appUsageDatabase = appUsageDatabase;
        }
        public async Task CalculateCO2eAsync(AppUsageModel appData)
            {

            var packageName = appData.PackageName;

            var conversionTable = await _conversionTableDatabase.GetConversionTable();

            if (conversionTable != null)
            {
                var conversionObject = conversionTable.Find(x => x.PackageName == packageName);
                Console.WriteLine(conversionObject.CO2Mins);
                Console.WriteLine("here");

                double CO2fromTable = conversionObject.CO2Mins;
                double CO2Mins = appData.UsageTimeMinutes;

                double CO2e = CO2fromTable * CO2Mins;
                Console.WriteLine(CO2e);
                 //add it to the appUsageModel?
            }
            
            //return CO2e;
        }
    }
    }
