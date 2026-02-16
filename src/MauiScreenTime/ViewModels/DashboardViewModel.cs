//using Android.AdServices.Common;
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
        private readonly ICO2Service _co2Service;
        public bool hasPermission;

        [ObservableProperty]
        private ObservableCollection<AppUsageModel> _appUsageList = new();


        public DashboardViewModel(IUsageStatsService usageStatsService, ICO2Service co2Service)
        {

            _usageStatsService = usageStatsService;
            _co2Service = co2Service;
            OnAppearing();

        }

        // Calls methods when page launches
        [RelayCommand]
        private async Task OnAppearing()
        {

            try
            {
                await GetUsageData();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calling the usage data in OnAppearing() dashboard: {ex}");

                await Shell.Current.DisplayAlert("Error", "Unable to load data. Please try again.","OK");
            }

            await GetCO2Coversion();
        }

        // gets usage data from service if permissions granted
        private async Task GetUsageData()
        {
            hasPermission = await _usageStatsService.HasPermissionAsync();
            if (hasPermission)
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
                    System.Diagnostics.Debug.WriteLine($"Error calling get usage data in dashboard: {ex}");

                    await Shell.Current.DisplayAlert("Error", "Unable to load data. Please try again.", "OK");
                }
            }

        }

        // return the appUsage with CO2e here to show in view 
        private async Task GetCO2Coversion()
        {

            foreach (var app in _appUsageList)
            {
                Console.WriteLine("here app into conversion dashboard");
                Console.WriteLine(app.ToString());
                var appData = await _co2Service.CalculateCO2eAsync(app);

                //_appUsageList.Add(appData);
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(appData));

            }
            _appUsageList.Add(appData);
        }
    }
}