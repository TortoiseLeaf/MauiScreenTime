using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiScreenTime.Data;

namespace MauiScreenTime.Services
{
    public interface IUsageStatsService
    {
        Task<bool> HasPermissionAsync();
        #if ANDROID
        Task<List<AppUsageModel>> GetAppUsageAsync();
        //void GetAppUsage();
#endif
        
    }
}
