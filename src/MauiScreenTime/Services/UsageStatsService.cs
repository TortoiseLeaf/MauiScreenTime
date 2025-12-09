using MauiScreenTime.Helpers;
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
        bool statusGranted;
        public async Task<bool> CheckAndRequestPermissionsAsync()
        {
            //public async void CheckAndRequestPermissionsAsync()
            //{
            //async Task RequestBluetooth()
            //{
            if (DeviceInfo.Platform != DevicePlatform.Android)
                Console.WriteLine("NOt on android error");

            var status = PermissionStatus.Unknown;

            if (DeviceInfo.Version.Major >= 12)
                Console.WriteLine("fires version >= 12");

            {
                status = await Permissions.CheckStatusAsync<UsageStatsPermission>();
                Console.WriteLine("fires status is assigned by checkstatusasync: ", status.ToString());


                if (status == PermissionStatus.Granted)
                {

                    statusGranted = true;
                    Console.WriteLine("fires status granted true ", statusGranted);
                    return statusGranted;
                }
                if (status == PermissionStatus.Denied)
                {
                    statusGranted = false;
                    Console.WriteLine("fires status granted false ", statusGranted);
                    status = await Permissions.RequestAsync<UsageStatsPermission>();
                }

                if (Permissions.ShouldShowRationale<UsageStatsPermission>())
                {
                    await Shell.Current.DisplayAlert("Needs permissions", "BECAUSE!!!", "OK");
                }

                status = await Permissions.RequestAsync<UsageStatsPermission>();

            }
            //else
            //{
            //    status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            //    if (status == PermissionStatus.Granted)
            //        //return;

            //    if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
            //    {
            //        await Shell.Current.DisplayAlert("Needs permissions", "BECAUSE!!!", "OK");
            //    }

            //    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();


            //}


            if (status != PermissionStatus.Granted)
                await Shell.Current.DisplayAlert("Permission required",
                    "Location permission is required for bluetooth scanning. " +
                    "We do not store or use your location at all.", "OK");

            // Here i want to open the settings page
            OpenUsageAccessSettings();

            status = await Permissions.RequestAsync<UsageStatsPermission>();
            Console.WriteLine("fires requestAsync");
            return statusGranted;
        }


 

        public async Task<bool> HasPermissionAsync()
        {
            Console.WriteLine("HaspermissionAsync fires");

#if ANDROID
                try
            {
                var appOpsManager = (AppOpsManager)Android.App.Application.Context.GetSystemService(Context.AppOpsService);
                var mode = appOpsManager.CheckOpNoThrow(
                    AppOpsManager.OpstrGetUsageStats,
                    Android.OS.Process.MyUid(),
                    Android.App.Application.Context.PackageName);

                     if (appOpsManager != null)
            System.Diagnostics.Debug.WriteLine("AppOpsNotNull fires: ", appOpsManager);



            System.Diagnostics.Debug.WriteLine("HaspermissionAsync fires: ", mode);

                return await Task.FromResult(mode == AppOpsManagerMode.Allowed);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking permission: {ex.Message}");
                return await Task.FromResult(false);
            }
#else
            Console.WriteLine("ERror");
            return await Task.FromResult(false).ConfigureAwait(false);

#endif
        }


        private void OpenUsageAccessSettings()
        {
#if ANDROID
            try
            {
                var intent = new Intent(Settings.ActionUsageAccessSettings);
                intent.SetFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening settings: {ex.Message}");
            }
#endif
        }

    }
    }



//    private async Task<bool> CheckPermissions()
//    {
//        PermissionStatus status = PermissionStatus.Unknown;

//        if (DeviceInfo.Platform == DevicePlatform.Android)
//        {

//            status = await Permissions.CheckStatusAsync<UsageStatsPermission>();

//            if (status == PermissionStatus.Granted)
//                return true;

//            if (Permissions.ShouldShowRationale<PackagePermission.PackageUsageStatsPermission>())
//            {
//                await Shell.Current.DisplayAlert("Needs permissions", "You must activate manualy", "OK");
//            }

//            status = await Permissions.RequestAsync<PackagePermission.PackageUsageStatsPermission>();
//        }

//        return status == PermissionStatus.Granted;
//    }

//}
//}

// Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.StartActivity(new Android.Content.Intent(Android.Provider.Settings.ActionUsageAccessSettings));