using MauiScreenTime.Data;
using MauiScreenTime.Services.Interfaces;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Microsoft.Maui.ApplicationModel.Permissions;


#if ANDROID
using Android.Content.PM;
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
        public List<string> appWhiteList = ["com.zhiliaoapp.musically", "com.reddit.frontpage", "com.facebook.katana", "com.instagram.android", "com.twitter.android", "tv.twitch.android.app", "com.snapchat.android", "com.pinterest", "com.linkedin.android"]; //"com.google.android.youtube", "package.android.youtube"];
        private Task<List<AppUsageModel>>? usageData;

#if ANDROID
        private Android.Content.Context _context;
#endif

        // opens device permissions page if permissions not granted
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

        // checks for permissions
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

        // gets installed packages from device
        public IList<string> GetInstalledPackages()
        {
            var installedWhitelistPackageNames = new List<string>();

#if ANDROID
            try
            {
                _context = Android.App.Application.Context;

                var packageManager = _context.PackageManager;
                var packages = packageManager.GetInstalledPackages(PackageInfoFlags.MatchAll);

                foreach (var package in packages)
                {
                    foreach (var app in appWhiteList)
                    {
                        if (app == package.PackageName)
                        {

                            installedWhitelistPackageNames.Add(package.PackageName);
                        }
                    }
                }
                return installedWhitelistPackageNames;

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting installed whitelist apps: ", Ex.Message);

            }
#endif
            return installedWhitelistPackageNames;
        }


        private long GetTodayMidnightMillis()
        {
            var localTimeZone = TimeZoneInfo.Local;
            var midnight = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
            return new DateTimeOffset(midnight, localTimeZone.GetUtcOffset(midnight))
                .ToUnixTimeMilliseconds();
        }

        // get app usage data from installed whitelisted apps
        public async Task<List<AppUsageModel>> GetAppUsageAsync()
        {

            IList<string> installedWhitelistPackageNames = GetInstalledPackages();

            return await Task.Run(() =>

            {
#if ANDROID
                _context = Android.App.Application.Context;
                var usageStatsManager = (UsageStatsManager)_context.GetSystemService(Context.UsageStatsService);

                //long startTimeMillis = GetTodayMidnightMillis();
                //long endTimeMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                 DateTime endTime = DateTime.Now;
                DateTime startTime = DateTime.Today;

                 long startTimeMillis = new DateTimeOffset(startTime.ToUniversalTime()).ToUnixTimeMilliseconds();
            long endTimeMillis = new DateTimeOffset(endTime.ToUniversalTime()).ToUnixTimeMilliseconds();

                 var usageStatsList = usageStatsManager.QueryUsageStats(
                    UsageStatsInterval.Daily,
                    startTimeMillis,
                    endTimeMillis    

            );

                var DeviceAppUsageList = new List<AppUsageModel>();

                if (usageStatsList != null)
                {
                    foreach (var usageObj in usageStatsList)
                    {

                        if (usageObj.TotalTimeInForeground > 0 &&
    installedWhitelistPackageNames.Contains(usageObj.PackageName) &&
    usageObj.LastTimeStamp >= startTimeMillis &&
    usageObj.FirstTimeStamp >= startTimeMillis)
{

                        var firstTime = DateTimeOffset.FromUnixTimeMilliseconds(usageObj.FirstTimeStamp).ToLocalTime();
            var lastTime = DateTimeOffset.FromUnixTimeMilliseconds(usageObj.LastTimeStamp).ToLocalTime();
            
            System.Diagnostics.Debug.WriteLine(
                $"PKG: {usageObj.PackageName} | " +
                $"First: {firstTime:yyyy-MM-dd HH:mm:ss} | " +
                $"Last: {lastTime:yyyy-MM-dd HH:mm:ss} | " +
                $"TotalMs: {usageObj.TotalTimeInForeground} | " +
                $"StartFilter: {DateTimeOffset.FromUnixTimeMilliseconds(startTimeMillis).ToLocalTime():yyyy-MM-dd HH:mm:ss}"
            );


            var existing = DeviceAppUsageList
    .FirstOrDefault(x => x.PackageName == usageObj.PackageName);

if (existing != null)
{
    existing.UsageTimeMilliseconds += TimeSpan.FromMilliseconds(usageObj.TotalTimeInForeground);
    existing.UsageTimeMinutes += usageObj.TotalTimeInForeground / 60000;
}
else 
{
                            DeviceAppUsageList.Add(new AppUsageModel
                            {
                                PackageName = usageObj.PackageName,
                                UsageTimeMilliseconds = TimeSpan.FromMilliseconds(usageObj.TotalTimeInForeground),
                                UsageTimeMinutes = usageObj.TotalTimeInForeground / 60000,

                            });
                        }
                    }
                } 
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Here app usage defaulted");

                    DeviceAppUsageList.Add(new AppUsageModel
                    {
                        PackageName = "Default",
                        UsageTimeMilliseconds = TimeSpan.FromMilliseconds(0),
                        UsageTimeMinutes = 0,

                    });
                }

                    var usageData = DeviceAppUsageList.OrderByDescending(a => a.UsageTimeMinutes).ToList();
#endif
                return usageData;

            });
        }
    }
}


