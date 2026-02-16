using MauiScreenTime.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Services
{
    public interface ICO2Service
    {
        Task<AppUsageModel> CalculateCO2eAsync(AppUsageModel appData);

    }
}
