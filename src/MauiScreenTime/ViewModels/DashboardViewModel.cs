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

        }
    }
}