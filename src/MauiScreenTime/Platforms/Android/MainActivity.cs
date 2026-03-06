using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Work;
using MauiScreenTime.Platforms.Android;

namespace MauiScreenTime
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Android.OS.Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // build factory before scheduled method runs
            var factory = IPlatformApplication.Current?.Services
                            .GetRequiredService<MauiWorkerFactory>();

            var config = new Configuration.Builder()
                .SetWorkerFactory(factory)
                .Build();

           // WorkManager.Initialize(this, config);

            WorkManagerHelper.ScheduleCalculateTotalCO2(this);
        }
    }
}
