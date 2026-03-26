using CommunityToolkit.Mvvm.ComponentModel;
using MauiScreenTime.Data.Interfaces;
using MauiScreenTime.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.ViewModels
{
    public partial class GoalViewModel : ObservableObject
    {
        private readonly IUserActivityLogDatabase _userActivityLogDatabase;
        private readonly ICO2Service _co2Service;

        [ObservableProperty]
        private double _co2TotalReduced = new();
        [ObservableProperty]
        private double _co2ReducedProgress = new();
        [ObservableProperty]
        private int _treesTotal = new();
        [ObservableProperty]
        private double _co2TotalDayBefore = new();
        [ObservableProperty]
        private double _co2TotalY = new();
 

        public GoalViewModel(ICO2Service co2Service, IUserActivityLogDatabase userActivityLogDatabase)
        {

            _userActivityLogDatabase = userActivityLogDatabase;
            _co2Service = co2Service;


            _ = InitialiseMethods();

            _ = GetDataSoFar();
            _ = DisplayProgressBar();
        }

        public async Task InitialiseMethods()
        {
            //await CalculateAndStoreDifference();
            
            await GetAndUpdateCO2ProgressBar();

            await RunGetAndUpdateCO2OnceADay();

            
            await GetTreesPlanted();
        }

        //public async Task CalculateAndStoreDifference()
        //{
        //    await _co2Service.CalculateAndStoreCO2DifferenceAsync();

        //}



        public async Task DisplayProgressBar()
        {
            var reduced = await _userActivityLogDatabase.GetTotalProgressBar();
            Co2ReducedProgress = reduced;

            System.Diagnostics.Debug.WriteLine("here progressbar runs" + Co2ReducedProgress);
        }


        // update Progress bar once a day
        private async Task RunGetAndUpdateCO2OnceADay()
        {
            var lastRun = Preferences.Get("LastRunDate", DateTime.MinValue.ToString());
            var lastRunDate = DateTime.Parse(lastRun);

            if (lastRunDate.Date < DateTime.Today)
            {
                System.Diagnostics.Debug.WriteLine("here progressbar runs");

                await _co2Service.CalculateAndStoreCO2DifferenceAsync();

                await _userActivityLogDatabase.UpdateProgressBar();

                //Co2ReducedProgress = await _userActivityLogDatabase.GetLatestProgressBar();
                Preferences.Set("LastRunDate", DateTime.Now.ToString());
            }
        }

        public async Task GetAndUpdateCO2ProgressBar()
        {

            await _userActivityLogDatabase.UpdateProgressBar();


        }

        public async Task GetTreesPlanted()
        {

            TreesTotal = await _userActivityLogDatabase.GetLatestTreesByDate(DateTime.Now);

        }

        public async Task GetDataSoFar()
        {
            var yesterday = DateTime.Now.Date.AddDays(-1);
            var dayBeforeYesterday = DateTime.Now.Date.AddDays(-2);

            var TodayTotalCO2Obj = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(DateTime.Now);


            var yesterdayTotalCO2Obj = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(yesterday);
            var dayBeforeYesterdayTotalCO2Obj = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(dayBeforeYesterday);


            Co2TotalY = yesterdayTotalCO2Obj.CO2Total;
            Co2TotalDayBefore = dayBeforeYesterdayTotalCO2Obj.CO2Total;

            Co2TotalReduced = await _userActivityLogDatabase.GetCO2TotalReduced();

        }
    }
}