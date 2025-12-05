using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using MauiScreenTime.Data;
using MauiScreenTime.ViewModels;
using MauiScreenTime.Pages;


namespace MauiScreenTime
{
    // Hi this is code
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

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "consent.db3");
            builder.Services.AddSingleton(s => new ConsentDatabase(dbPath));
            builder.Services.AddSingleton(s => new ConversionTableDatabase());


            //builder.Services.AddSingleton<App>();

            builder.Services.AddTransient<ConsentViewModel>();
            builder.Services.AddTransient<ConsentPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
