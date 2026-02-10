using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using MauiScreenTime.Data;
using MauiScreenTime.ViewModels;
using MauiScreenTime.Pages;
using MauiScreenTime.Services;
using Serilog;
using Serilog.Extensions.Hosting;
using Serilog.Sinks.File;

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

            builder.Services.AddSerilog(
        new LoggerConfiguration()
            .WriteTo.File(Path.Combine(FileSystem.Current.AppDataDirectory, "log.txt"))
            .CreateLogger()
    );

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


            builder.Services.AddLogging();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
