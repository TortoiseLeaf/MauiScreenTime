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

           
            //await CalculateCO2DifferenceAsync();

            // debug
            //await IncrementTrees();
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
                
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error totalling CO2e in dashboard: {ex}");

                await Shell.Current.DisplayAlert("Error", "Unable to total CO2e.", "OK");
            }


        }

        //public async Task IncrementTrees()
        //{
        //    try
        //    {
        //        //await _userActivityLogDatabase.AddTrees(1);
        //        await _userActivityLogDatabase.AddActivityLog(0, 0, 1);

        //        var x = await _userActivityLogDatabase.GetLatestTreesByDate(DateTime.Today);
        //        Console.WriteLine("Tree object added successfully here");
        //        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(x));


        //    } catch (Exception ex)
        //    {
        //        Console.WriteLine("Error adding trees from dashboard: " + ex.Message);

        //    }
            

        //}

        public async Task GetDataSoFar()
        {
            var today = DateTime.Now.Date;
            var yesterday = DateTime.Now.Date.AddDays(-1);

            var yesterdayTotalCO2Obj = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(yesterday);
            var todayTotalCO2Obj = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(today);

            LatestTrees = await _userActivityLogDatabase.GetLatestTreesByDate(today);

            Co2TotalReduced = await _userActivityLogDatabase.GetCO2TotalReducedByDate(DateTime.Now);

            Co2TotalY = yesterdayTotalCO2Obj.CO2Total;
            Co2TotalTD = todayTotalCO2Obj.CO2Total;


        }

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

//        public async Task<double> CalculateCO2DifferenceAsync()
//        {

//            double todayCO2Total;
//            double yesterdayCO2Total;
//            double differenceSaved = 0;


//            var todayTotalCO2Obj = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(DateTime.Now);
//            todayCO2Total = todayTotalCO2Obj.CO2Total;

//            var yesterdayTotalCO2Obj = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(DateTime.Now.AddDays(-1));
//            yesterdayCO2Total = yesterdayTotalCO2Obj.CO2Total;

//            //// debug to show yesterday and todays entries with the highest CO2Total
//            //var today = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(DateTime.Now);
//            //var yesterday = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(DateTime.Now.AddDays(-1));

//            //Console.WriteLine("here calculate diff");
//            //Console.WriteLine(todayCO2Total);
//            //Console.WriteLine(yesterdayCO2Total);
//            //Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(today));
//            //Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(yesterday));


//            if (yesterdayCO2Total > 0)
//            {
//                differenceSaved = yesterdayCO2Total - todayCO2Total;

//                if (differenceSaved > 0)
//                {
//                    Console.WriteLine("This is the difference saved: " + differenceSaved);

//                    try
//                    {
//                        await _userActivityLogDatabase.AddActivityLog(0, differenceSaved,0);
//                        Console.WriteLine("diff saved successfully");

//                    }
//                    catch (Exception ex)
//                    {
//                        System.Diagnostics.Debug.WriteLine("Error adding diff: ", ex.Message);

//                    }

//#if ANDROID
//                    Log.Debug("CO2Service", "here successfully CO2 difference saved to log");
//#endif

//                }
//            }
            
//            return differenceSaved;
//        }

    }
}