using CommunityToolkit.Mvvm.Input;
using MauiScreenTime.Services;

namespace MauiScreenTime.ViewModels
{
    public partial class DashboardViewModel
    {
        private readonly IUsageStatsService _usageStatsService;

        public DashboardViewModel(IUsageStatsService usageStatsService) //UsageDatabase db)
        {
            //_db = db;
            _usageStatsService = usageStatsService;

            PageAppearing();
        }

        [RelayCommand]
        private async Task PageAppearing()
        {
            await _usageStatsService.HasPermissionAsync();
            usageGo();

        }

        public async void usageGo()
        {
            bool hasPerm = await _usageStatsService.HasPermissionAsync();

            if (hasPerm)
            {

                try
                {
#if ANDROID

                    _usageStatsService.GetAppUsage();
#endif
                }
                catch (Exception ex)
                {
                    Console.WriteLine("log error firing getappusage: ", ex.Message.ToString());
                }
            }
        }
    }
}