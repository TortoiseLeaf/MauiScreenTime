using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiScreenTime.Data;
using MauiScreenTime.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace MauiScreenTime.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        protected readonly ILogger<DashboardViewModel> _logger;
        private readonly IUsageStatsService _usageStatsService;
        public bool _hasPermission;

        [ObservableProperty]
        private ObservableCollection<AppUsageModel> _appUsageList = new();


        public DashboardViewModel(IUsageStatsService usageStatsService, ILogger<DashboardViewModel> logger)
        {

            _usageStatsService = usageStatsService;

            _ = OnAppearing();

            _logger = logger;
            _logger.LogInformation("xxx ViewModel constructor called");
            Console.WriteLine("xxx");
            Console.WriteLine(_logger);

        }

        // methods that run when page loads
        [RelayCommand]
        private async Task OnAppearing()
        {
            _logger.LogInformation("xxx I did the thing!");
            try
            {
                throw new InvalidOperationException("xxx Such bad, much error.");
                await GetUsageData();
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting usage data in Dashboard viewmodel.");
                //await Application.Current.MainPage.DisplayAlert("Logger Test", $"Exception caught in dashboard viewmodel: {ex.Message}", "OK");
            }
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