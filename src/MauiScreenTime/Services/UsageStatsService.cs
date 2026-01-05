using MauiScreenTime.Data;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Maui.ApplicationModel.Permissions;

#if ANDROID
using Android.App;
using Android.App.Usage;
using Android.Content;
using Android.Provider;
using MauiScreenTime;
using MauiScreenTime.Services;
using Xamarin.Google.Crypto.Tink.Signature;
using AppOpsMode = Android.App.AppOpsManagerMode;
#endif

namespace MauiScreenTime.Services
{
    public class UsageStatsService : IUsageStatsService
    {

        public void OpenUsageAccessSettings()
        {
#if ANDROID
            try
            {
                Platform.CurrentActivity.StartActivity(new Android.Content.Intent(Android.Provider.Settings.ActionUsageAccessSettings));

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening settings: {ex.Message}");
            }
#endif
        }
        public async Task<bool> HasPermissionAsync()
        {

#if ANDROID
            try
            {
                var appOpsManager = (AppOpsManager)Android.App.Application.Context.GetSystemService(Context.AppOpsService);
                var mode = appOpsManager.CheckOpNoThrow(
                    AppOpsManager.OpstrGetUsageStats,
                    Android.OS.Process.MyUid(),
                    Android.App.Application.Context.PackageName);

                await Task.FromResult(mode == AppOpsManagerMode.Allowed);

                if (mode == AppOpsManagerMode.Allowed)
                {
                    Console.WriteLine("log permission granted");
                }
                else
                {
                    Console.WriteLine("log permission denied");
                    await Shell.Current.DisplayAlert("Permission required",
                   "Permission is required for app tracking. " +
                   "We do not store this data, only the calculations made from it.", "OK");
                    OpenUsageAccessSettings();
                }
                return await Task.FromResult(mode == AppOpsManagerMode.Allowed);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking permission: {ex.Message}");

                return await Task.FromResult(false);
            }
#else
                    return await Task.FromResult(false).ConfigureAwait(false);

#endif

        }


        // check out scope for android tag, it's weird to have it in the IUsageStats interface. might even be a weakness?
#if ANDROID

        public async Task<List<AppUsageModel>> GetAppUsageAsync()
        {
        return await Task.Run(() =>
        {
            var context = Android.App.Application.Context;
            var usageStatsManager = (UsageStatsManager)context.GetSystemService(Context.UsageStatsService);

            // interval starts at midnight today, ends with right now
            DateTime endTime = DateTime.Now;
            DateTime startTime = DateTime.Today;

            long startTimeMillis = new DateTimeOffset(startTime).ToUnixTimeMilliseconds();
            long endTimeMillis = new DateTimeOffset(endTime).ToUnixTimeMilliseconds();

            var usageStatsList = usageStatsManager.QueryUsageStats(
                UsageStatsInterval.Daily,
                startTimeMillis,
                endTimeMillis
            );

            var DeviceAppUsageList = new List<AppUsageModel>();

            if (usageStatsList != null)
            {
                foreach (var appUsageData in usageStatsList)
                {
                    
                    if (appUsageData.TotalTimeInForeground > 0)
                    {
                        DeviceAppUsageList.Add(new AppUsageModel
                        {
                            PackageName = appUsageData.PackageName,
                            AppName =  GetAppName(context, appUsageData.PackageName),
                            UsageTimeMilliseconds = TimeSpan.FromMilliseconds(appUsageData.TotalTimeInForeground),
                            
                        });
                    }
                }
            }

            var usageData = DeviceAppUsageList.OrderByDescending(a => a.UsageTimeMilliseconds).ToList();

            return usageData;
            });
        }




        // replace this with hardcoded app names? "com.gmail" = "Gmail" e.g.
        private string GetAppName(Context context, string packageName)
        {
            try
            {
                var packageManager = context.PackageManager;
                var applicationInfo = packageManager.GetApplicationInfo(packageName, 0);
                return packageManager.GetApplicationLabel(applicationInfo);
            }
            catch
            {
                return packageName;
            }
        }

#endif


    }
}