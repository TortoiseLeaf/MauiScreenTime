using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiScreenTime.Data;
using MauiScreenTime.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace MauiScreenTime.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IUsageStatsService _usageStatsService;
        public bool _hasPermission;

        [ObservableProperty]
        private ObservableCollection<AppUsageModel> _appUsageList = new();


        public DashboardViewModel(IUsageStatsService usageStatsService)
        {

            _usageStatsService = usageStatsService;

            _ = OnAppearing();

        }

        // methods that run when page loads
        [RelayCommand]
        private async Task OnAppearing()
        {

            await GetUsageData();

        }


        private async Task GetUsageData()
        {
            _hasPermission = await _usageStatsService.HasPermissionAsync();
            if (_hasPermission)
            {
                try
                {
                    var usageData = await _usageStatsService.GetAppUsageAsync();

                    _appUsageList.Clear();
                    foreach (var app in usageData)
                    {
                        _appUsageList.Add(app);
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error collecting data", ex.Message, "OK");
                }
            }

        }
    }
}