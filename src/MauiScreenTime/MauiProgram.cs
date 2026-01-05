using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using MauiScreenTime.Data;
using MauiScreenTime.ViewModels;
using MauiScreenTime.Pages;
using MauiScreenTime.Services;



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

            builder.Services.AddSingleton<ConsentDatabase>();
            builder.Services.AddSingleton(s => new ConversionTableDatabase());
            builder.Services.AddSingleton<IStartupService, StartupService>();

            builder.Services.AddSingleton<IUsageStatsService, UsageStatsService>();

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
