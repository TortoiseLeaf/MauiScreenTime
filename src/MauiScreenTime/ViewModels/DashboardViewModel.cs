//using Android.AdServices.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiScreenTime.Data;
using MauiScreenTime.Services.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
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
        private List<AppUsageModel> _appUsageList = new();
        [ObservableProperty]
        private ObservableCollection<AppUsageModel> _appUsageListCO2 = new();
        [ObservableProperty]
        private double _co2Total = new();
        [ObservableProperty]
        private double _co2DailySaved = new();

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

            await GetCO2Total();

            await GetDailyDifference();
        }

        // gets usage data from service if permissions granted
        public async Task GetUsageData()
        {
            hasPermission = await _usageStatsService.HasPermissionAsync();
            if (hasPermission)
            {
                try
                {
                    var usageData = await _usageStatsService.GetAppUsageAsync();

                    if (usageData != null)
                    {
                        _appUsageList.Clear();
                        foreach (var app in usageData)
                        {
                            _appUsageList.Add(app);
                        }
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
        public async Task GetCO2Coversion()
        {

            foreach (var app in _appUsageList)
            {
                // try/catch
                var appData = await _co2Service.CalculateCO2eAsync(app);

                _appUsageListCO2.Add(appData);

            }
            //_appUsageList.Add(appData);
        }

        // return totalCO2
        public async Task GetCO2Total()
        {


            try
            {
                Co2Total = await _co2Service.CalculateCO2TotalAsync(_appUsageList);
                //Console.WriteLine("here dashboard value");
                //Console.WriteLine(_co2Total);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error totalling CO2e in dashboard: {ex}");

                await Shell.Current.DisplayAlert("Error", "Unable to total CO2e.", "OK");
            }


            //return _co2Total; // this isn't showing, why?
        }

        public async Task GetDailyDifference()
        {


            try
            {
                _co2DailySaved = await _co2Service.CalculateCO2DifferenceAsync();
                if (_co2DailySaved < 0)
                {
                    Co2DailySaved = 0;
                }
                Console.WriteLine("here dashboard dailysaved value");
                Console.WriteLine(_co2DailySaved);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error totalling CO2e in dashboard: {ex}");

                await Shell.Current.DisplayAlert("Error", "Unable to total CO2e.", "OK");
            }


            //return _co2DailySaved ;
        }

    }
}