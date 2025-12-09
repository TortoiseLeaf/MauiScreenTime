using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Services
{
    public interface IUsageStatsService
    {
        Task<bool> CheckAndRequestPermissionsAsync();
        //Task<bool> HasPermissionAsync();
       // Task<List<AppUsageData>> GetAppUsageAsync();
    }
}
