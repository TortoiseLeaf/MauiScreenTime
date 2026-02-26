using MauiScreenTime.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Services.Interfaces
{
    public interface ICO2Service
    {
        Task<AppUsageModel> CalculateCO2eAsync(AppUsageModel appData);

        Task<double> CalculateCO2TotalAsync(List<AppUsageModel> appUsageList);

        Task<double> CalculateCO2DifferenceAsync();

    }
}
