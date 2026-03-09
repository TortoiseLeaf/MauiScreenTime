//using Android.AdServices.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiScreenTime.Data;
using MauiScreenTime.Data.Interfaces;
using MauiScreenTime.Services.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.Json;
#if ANDROID
using Android.Util;
#endif

namespace MauiScreenTime.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IUsageStatsService _usageStatsService;
        private readonly IUserActivityLogDatabase _userActivityLogDatabase;
        private readonly ICO2Service _co2Service;
        public bool hasPermission;

        [ObservableProperty]
        private List<AppUsageModel> _appUsageList = new();
        [ObservableProperty]
        private ObservableCollection<AppUsageModel> _appUsageListCO2 = new();
        [ObservableProperty]
        private double _co2Total = new();
        [ObservableProperty]
        private double _co2TotalReduced = new();

        [ObservableProperty]
        private double _co2DailySavedDebug = new();
        [ObservableProperty]
        private double _co2TotalTD = new();
        [ObservableProperty]
        private double _co2TotalY = new();
        [ObservableProperty]
        private double _latestTrees = new();

        public DashboardViewModel(IUsageStatsService usageStatsService, ICO2Service co2Service, IUserActivityLogDatabase userActivityLogDatabase)
        {

            _usageStatsService = usageStatsService;
            _userActivityLogDatabase = userActivityLogDatabase;
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
            //await GetAllActivity();
            
            await GetCO2Coversion();

            await GetCO2Total();

            await CalculateDifference();

            // returns totals to frontend
            await GetDataSoFar();
            
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
#if ANDROID
                    Log.Debug("DashboardVM", "Error calling get usage data in dashboard");
#endif


                    await Shell.Current.DisplayAlert("Error", "Unable to load data. Please try again.", "OK");
                }
            }

        }

        public async Task GetCO2Coversion()
        {

            foreach (var app in _appUsageList)
            {
                // try/catch
                var appData = await _co2Service.CalculateCO2eAsync(app);

                _appUsageListCO2.Add(appData);

            }
        }

        public async Task GetCO2Total()
        {

            try
            {
                // returns it directly on the fly
                Co2Total = await _co2Service.CalculateCO2TotalAsync(_appUsageList);

                // save todays total to db
                await _userActivityLogDatabase.AddActivityLog(Co2Total, 0, 0);
                
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error totalling CO2e in dashboard: {ex}");

                await Shell.Current.DisplayAlert("Error", "Unable to total CO2e.", "OK");
            }


        }

        public async Task GetDataSoFar()
        {
            var yesterday = DateTime.Now.Date.AddDays(-1);
            var dayBeforeYesterday = DateTime.Now.Date.AddDays(-2);


            var yesterdayTotalCO2Obj = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(yesterday);
            var dayBeforeYesterdayTotalCO2Obj = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(dayBeforeYesterday);

            LatestTrees = await _userActivityLogDatabase.GetLatestTreesByDate(dayBeforeYesterday);

            Co2TotalReduced = await _userActivityLogDatabase.GetCO2TotalReduced();

            Co2TotalY = yesterdayTotalCO2Obj.CO2Total;
            Co2TotalTD = dayBeforeYesterdayTotalCO2Obj.CO2Total;


        }

        public async Task CalculateDifference()
        {
            await _co2Service.CalculateCO2DifferenceAsync();
        }

        // just been using this for debugging
        public async Task GetAllActivity()
        {
            //Console.WriteLine("all logs here");

            //// add data to entries for debugging
            //await _userActivityLogDatabase.AddActivityLog(0, 10, 0);
            //await _userActivityLogDatabase.AddActivityLog(50, 0, 0);


            //var xy = await _userActivityLogDatabase.GetAllActivitiesLogged();
            //foreach (var i in xy) 
            //{
            //    if (i.Date == DateTime.Today)
            //    {
            //        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(i));
            //    }
            //}

            //var yesterdayData = await _userActivityLogDatabase.GetActivityByDate(DateTime.Now.AddDays(-1));
            //var todayData = await _userActivityLogDatabase.GetActivityByDate(DateTime.Now);
            //Console.WriteLine("today and yesterdays data");
            //Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(todayData));
            //Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(yesterdayData));
        }


    }
}