using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiScreenTime.Data;
using MauiScreenTime.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace MauiScreenTime.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IUsageStatsService _usageStatsService;
        public bool hasPermission;

        [ObservableProperty]
        private ObservableCollection<AppUsageModel> appUsageData = new();


        public DashboardViewModel(IUsageStatsService usageStatsService)
        {
            
            _usageStatsService = usageStatsService;

            OnAppearing();

        }

        [RelayCommand]
        private async Task OnAppearing()
        {
            await _usageStatsService.HasPermissionAsync();
            await collectAppUsage();
            
        }

        public async Task collectAppUsage()
        {
            hasPermission = await _usageStatsService.HasPermissionAsync();

            if (hasPermission)
            {

                try
                {
#if ANDROID
                var usageData = await _usageStatsService.GetAppUsageAsync();
                AppUsageData = new ObservableCollection<AppUsageModel>(usageData);
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