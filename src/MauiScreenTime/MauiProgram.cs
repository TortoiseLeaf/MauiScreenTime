using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using MauiScreenTime.Data;
using MauiScreenTime.Data.Interfaces;
using MauiScreenTime.ViewModels;
using MauiScreenTime.Pages;
using MauiScreenTime.Services;
using MauiScreenTime.Services.Interfaces;


namespace MauiScreenTime
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IConsentDatabase, ConsentDatabase>();
            builder.Services.AddSingleton<IAppUsageDatabase, AppUsageDatabase>();
            builder.Services.AddSingleton<IConversionTableDatabase, ConversionTableDatabase>();
            builder.Services.AddSingleton<IUserActivityLogDatabase, UserActivityLogDatabase>();
            

            builder.Services.AddSingleton<IStartupService, StartupService>();
            builder.Services.AddSingleton<ICO2Service, CO2Service>();
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
            builder.Services.AddSingleton<IUsageStatsService, UsageStatsService>();
            builder.Services.AddSingleton<IDailyWorkerService, DailyWorkerService>();

#if ANDROID
            builder.Services.AddSingleton<MauiScreenTime.Platforms.Android.MauiWorkerFactory>();
#endif

            builder.Services.AddSingleton<App>();
            
            //builder.Services.AddTransient<ConsentDatabase>();

            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<ConsentViewModel>();
            builder.Services.AddTransient<MainViewModel>();


            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<ConsentPage>();
            builder.Services.AddTransient<MainPage>();




#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
