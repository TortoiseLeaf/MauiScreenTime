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

            await GetCO2Coversion();

            await GetCO2Total();

            //await GetDailyDifference();

            await CalculateCO2DifferenceAsync();

            // debug
            await getDataSoFar();
            await GetAllActivity();
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
                // returns it directly on the fly
                Co2Total = await _co2Service.CalculateCO2TotalAsync(_appUsageList);
                
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error totalling CO2e in dashboard: {ex}");

                await Shell.Current.DisplayAlert("Error", "Unable to total CO2e.", "OK");
            }


        }

        public async Task GetDailyDifference()
        {


            try
            {
                
                System.Diagnostics.Debug.WriteLine("Co2DailySaved fetched to dashboard successfully");
#if ANDROID
                    Log.Debug("DashboardVM", "co2DailySaved fetched from log to dashboard successfully");
#endif

                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error totalling CO2e in dashboard: {ex}");

                await Shell.Current.DisplayAlert("Error", "Unable to total CO2e.", "OK");
            }
        }

        public async Task getDataSoFar()
        {
            var today = DateTime.Now.Date;
            var yesterday = DateTime.Now.Date.AddDays(-1);

            var yday = await _userActivityLogDatabase.GetHighestCO2TotalByDate(yesterday);
            var todayH = await _userActivityLogDatabase.GetHighestCO2TotalByDate(today);

            Co2TotalReduced = await _userActivityLogDatabase.GetCO2TotalReducedByDate(DateTime.Now);

            Co2TotalY = yday.CO2Total;
            Co2TotalTD = todayH.CO2Total;

            //var xy = await _userActivityLogDatabase.GetActivityByDate(today);
            //var y = await _userActivityLogDatabase.GetActivityByDate(yesterday);
            //Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(xy));
            //Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(y));


        }

       
        public async Task GetAllActivity()
        {
            //Console.WriteLine("all logs here");
            //Console.WriteLine("all logs here");
            //await _userActivityLogDatabase.AddActivityLogDEBUG(100, 10, null);


            //var xy = await _userActivityLogDatabase.GetAllActivitiesLogged();
            //foreach (var i in xy)
            //{
            //    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(i));

            //}

            //var yesterdayData = await _userActivityLogDatabase.GetActivityByDate(DateTime.Now.AddDays(-1));
            //var todayData = await _userActivityLogDatabase.GetActivityByDate(DateTime.Now);
            //Console.WriteLine("today and yesterdays data");
            //Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(todayData));
            //Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(yesterdayData));
        }

        public async Task<double> CalculateCO2DifferenceAsync()
        {

            double todayTotal;
            double yesterdayTotal;
            double differenceSaved;
            var tdyT = await _userActivityLogDatabase.GetHighestCO2TotalByDate(DateTime.Now);
            todayTotal = tdyT.CO2Total;


            var ydyT = await _userActivityLogDatabase.GetHighestCO2TotalByDate(DateTime.Now.AddDays(-1));
            yesterdayTotal = ydyT.CO2Total;

            var today = await _userActivityLogDatabase.GetActivityByDate(DateTime.Now);
            var yesterday = await _userActivityLogDatabase.GetActivityByDate(DateTime.Now.AddDays(-1));

            Console.WriteLine("here calculate diff");
            Console.WriteLine(todayTotal);
            Console.WriteLine(yesterdayTotal);
            //Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(today));
            //Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(yesterday));

            differenceSaved = yesterdayTotal - todayTotal;


            if (differenceSaved > 0)
            {
                Console.WriteLine(differenceSaved);

                var x = await _userActivityLogDatabase.GetActivityByDate(DateTime.Now);
                Console.WriteLine("here co2saved before");
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(x));


                //await _userActivityLogDatabase.AddCO2SavedDaily(differenceSaved);

                // problematic because if the 0 overwrites you get inconsistent data
                try
                {
                    await _userActivityLogDatabase.AddActivityLog(0, differenceSaved, 0);
                } catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error adding diff: ", ex.Message);

                }

                var xy = await _userActivityLogDatabase.GetActivityByDate(DateTime.Now);
                Console.WriteLine("here co2saved after");
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(xy));
                System.Diagnostics.Debug.WriteLine("CO2 difference daily saved to activityLog successfully!");

#if ANDROID
                Log.Debug("CO2Service", "here successfully CO2 difference saved to log");
#endif

            }
            // [DOTNET] {"Id":154,"Date":"2026-03-02T00:00:00","TimeStamp":"2026-03-02T16:25:04.8102844","CO2Total":0,"CO2SavedDaily":0,"TreesPlanted":0}

            var xyz = await _userActivityLogDatabase.GetActivityByDate(DateTime.Now);
            Console.WriteLine("here co2saved today");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(xyz));
            return differenceSaved;
        }

    }
}