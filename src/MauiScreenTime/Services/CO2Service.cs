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
        public async Task CalculateCO2eAsync()
            {
            
                
            }
        }
    }
