//using Android.Media;
using MauiScreenTime.Data;
using MauiScreenTime.Data.Interfaces;
using MauiScreenTime.Pages;
using MauiScreenTime.Services.Interfaces;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if ANDROID
using Android.Content;
using Android.OS;
using Android.Provider;
#endif

namespace MauiScreenTime.Services
{

    public class StartupService : IStartupService
    {
        private readonly IConsentDatabase _consentDatabase;
        private bool _hasConsent;

        public StartupService(IConsentDatabase consentDatabase)
        {
            if (consentDatabase != null)
            {
                _consentDatabase = consentDatabase;
            }
            else
            {
                Shell.Current.GoToAsync(nameof(ConsentPage));

            }
        }

        // checks for policy consent boolean from dedicated db
        public async Task InitializeConsentCheckAsync()
        {

            _hasConsent = await _consentDatabase.HasConsent();

            if (_hasConsent)
            {
                await Shell.Current.GoToAsync(nameof(DashboardPage));
            }
            else
            {
                await Shell.Current.GoToAsync(nameof(ConsentPage));
            }
        }



        public bool IsIgnoringBatteryOptimizations()
        {
#if ANDROID
            try {

            var context = Android.App.Application.Context;
            var powerManager = (PowerManager)context.GetSystemService(Context.PowerService);
            return powerManager.IsIgnoringBatteryOptimizations(context.PackageName);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking battery optimizations: {ex}");
                return false;
            }
#endif
            return false;
        }


        public void RequestIgnoreBatteryOptimizations()
        {
#if ANDROID
            var context = Android.App.Application.Context;
            if (!IsIgnoringBatteryOptimizations())
            {
            try {
                var intent = new Intent(Settings.ActionRequestIgnoreBatteryOptimizations);
                intent.SetData(Android.Net.Uri.Parse($"package:{context.PackageName}"));
                intent.AddFlags(ActivityFlags.NewTask);
                context.StartActivity(intent);

                }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error requesting to ignore battery optimizations: {ex}");
            }
            }
#endif
        }
    }
}
