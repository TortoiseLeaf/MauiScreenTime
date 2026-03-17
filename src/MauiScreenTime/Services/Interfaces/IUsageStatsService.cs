using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiScreenTime.Data;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace MauiScreenTime.Services.Interfaces
{
    public interface IUsageStatsService
    {
        IList<string> GetInstalledPackages();
        Task<bool> HasPermissionAsync();
        
        Task<List<AppUsageModel>> GetAppUsageAsync();

        
    }
}
