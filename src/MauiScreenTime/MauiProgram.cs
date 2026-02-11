using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using MauiScreenTime.Data;
using MauiScreenTime.ViewModels;
using MauiScreenTime.Pages;
using MauiScreenTime.Services;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Extensions.Hosting;
using Serilog.Sinks.File;

namespace MauiScreenTime
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            var logPath = Path.Combine(FileSystem.AppDataDirectory, "logs.txt");
            builder.Services.AddSerilog(
new LoggerConfiguration()
.MinimumLevel.Debug() // remove this for prod
.WriteTo.File(Path.Combine(FileSystem.Current.AppDataDirectory, "log.txt"), rollingInterval: RollingInterval.Day)
.CreateLogger()
);
            Log.Information("Serilog initialized - log path: {LogPath}", logPath);

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });


            builder.Services.AddSingleton<ConsentDatabase>();
            builder.Services.AddSingleton(s => new ConversionTableDatabase());
            builder.Services.AddSingleton<IStartupService, StartupService>();

            builder.Services.AddSingleton<IUsageStatsService, UsageStatsService>();
        
            builder.Services.AddSingleton<App>();
            

            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<ConsentViewModel>();
            builder.Services.AddTransient<MainViewModel>();


            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<ConsentPage>();
            builder.Services.AddTransient<MainPage>();


            builder.Services.AddLogging();
            Log.Information("App started successfully");

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
