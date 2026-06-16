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



        // get app usage data from installed whitelisted apps
        public async Task<List<AppUsageModel>> GetAppUsageAsync()
        {
            IList<string> installedWhitelistPackageNames = GetInstalledPackages();

            return await Task.Run(() =>
            {
#if ANDROID
        _context = Android.App.Application.Context;
        var usageStatsManager = (UsageStatsManager)_context.GetSystemService(Context.UsageStatsService);

        DateTime endTime = DateTime.Now;
        DateTime startTime = DateTime.Today;

        long startTimeMillis = new DateTimeOffset(startTime).ToUnixTimeMilliseconds();
        long endTimeMillis = new DateTimeOffset(endTime).ToUnixTimeMilliseconds();

        // Query events instead of usage stats
        var events = usageStatsManager.QueryEvents(startTimeMillis, endTimeMillis);
        var usageEvent = new UsageEvents.Event();

        // Track foreground time per app
        var appUsageTime = new Dictionary<string, long>();
        var appStartTimes = new Dictionary<string, long>();

        // Process all events
        while (events.HasNextEvent)
        {
            events.GetNextEvent(usageEvent);

            string packageName = usageEvent.PackageName;
            long timestamp = usageEvent.TimeStamp;

            // Only track whitelisted apps
            if (!installedWhitelistPackageNames.Contains(packageName))
                continue;


            if (UsageEventType.ActivityResumed == usageEvent.EventType) // App came to foreground
            {
                appStartTimes[packageName] = timestamp;
            }
            else if (UsageEventType.ActivityPaused == usageEvent.EventType) // App went to background
            {
                if (appStartTimes.ContainsKey(packageName))
                {
                    long duration = timestamp - appStartTimes[packageName];
                    if (!appUsageTime.ContainsKey(packageName))
                        appUsageTime[packageName] = 0;
                    appUsageTime[packageName] += duration;
                    appStartTimes.Remove(packageName);
                }
            }

            //switch (usageEvent.EventType)
            //{
            //    case UsageEventType.ActivityResumed: // App came to foreground
            //        appStartTimes[packageName] = timestamp;
            //        break;

            //    case UsageEventType.ActivityPaused: // App went to background
            //        if (appStartTimes.ContainsKey(packageName))
            //        {
            //            long duration = timestamp - appStartTimes[packageName];

            //            if (!appUsageTime.ContainsKey(packageName))
            //                appUsageTime[packageName] = 0;

            //            appUsageTime[packageName] += duration;
            //            appStartTimes.Remove(packageName);
            //        }
            //        break;
            //}
        }

        // Handle apps still in foreground
        foreach (var kvp in appStartTimes)
        {
            long duration = endTimeMillis - kvp.Value;

            if (!appUsageTime.ContainsKey(kvp.Key))
                appUsageTime[kvp.Key] = 0;

            appUsageTime[kvp.Key] += duration;
        }

        var DeviceAppUsageList = new List<AppUsageModel>();

        if (appUsageTime.Count > 0)
        {
            foreach (var app in appUsageTime)
            {
                if (app.Value > 0) // Only include apps with actual usage time
                {
                    DeviceAppUsageList.Add(new AppUsageModel
                    {
                        PackageName = app.Key,
                        UsageTimeMilliseconds = TimeSpan.FromMilliseconds(app.Value),
                        UsageTimeMinutes = app.Value / 60000,
                    });
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


