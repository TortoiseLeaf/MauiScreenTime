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
        }

        public async Task InitialiseMethods()
        {
            await CalculateDifferenceAndUpdateProgressBar();
            //await IncrementTree();
            await GetCO2ProgressBar();
            await GetTreesPlanted();
        }

        public async Task CalculateDifferenceAndUpdateProgressBar()
        {
            var co2ReducedToday = await _co2Service.CalculateAndStoreCO2DifferenceAsync();
            //System.Diagnostics.Debug.WriteLine("here totalreduced calculate diff fired");
            //System.Diagnostics.Debug.WriteLine(Co2TotalReduced);

            

            // don't need to do that here do that in the co2 method
            //await _userActivityLogDatabase.AddActivityLog(0, 0, co2ReducedProgress, 0);

        }

        public async Task GetCO2ProgressBar()
        {
            // gets reduced progress
            Co2ReducedProgress = await _userActivityLogDatabase.GetTotalCO2ReducedProgress();

            //// if it's > 200 update
            //if (Co2ReducedProgress >= 0 && Co2ReducedProgress >= 200)
            //{
            //    // Add tree
            //    await _userActivityLogDatabase.AddActivityLog(0, 0, 0, 1);
            //}
        }
        //public async Task IncrementTree()
        //{
        //    // HERE
        //    // this always calls total reduced to date so it will always update a tree as long as it's > 200
        //    // how to isolate the tree increment to just the progress bar?
        //    Co2ReducedProgress = await _userActivityLogDatabase.GetTotalCO2ReducedProgress();
        //    //Co2TotalReduced = await _userActivityLogDatabase.GetCO2TotalReduced();


        //    if (Co2ReducedProgress >= 0 && Co2ReducedProgress >= 200)
        //    {
        //        //System.Diagnostics.Debug.WriteLine("here totalreduced in increment tree2");
        //        //System.Diagnostics.Debug.WriteLine(Co2TotalReduced);

        //        await _userActivityLogDatabase.AddActivityLog(0, 0,0, 1);
        //    }

        //}
        public async Task GetTreesPlanted()
        {

            TreesTotal = await _userActivityLogDatabase.GetLatestTreesByDate(DateTime.Now);
            //System.Diagnostics.Debug.WriteLine("here treesplanted fired");
            //System.Diagnostics.Debug.WriteLine(TreesTotal);

        }

        public async Task GetDataSoFar()
        {
            var yesterday = DateTime.Now.Date.AddDays(-1);
            var dayBeforeYesterday = DateTime.Now.Date.AddDays(-2);

            var TodayTotalCO2Obj = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(DateTime.Now);


            var yesterdayTotalCO2Obj = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(yesterday);
            var dayBeforeYesterdayTotalCO2Obj = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(dayBeforeYesterday);

            //LatestTrees = await _userActivityLogDatabase.GetLatestTreesByDate(dayBeforeYesterday);

            //Co2TotalReduced = await _userActivityLogDatabase.GetCO2TotalReduced();

            Co2TotalY = yesterdayTotalCO2Obj.CO2Total;
            Co2TotalDayBefore = dayBeforeYesterdayTotalCO2Obj.CO2Total;

            Co2TotalReduced = await _userActivityLogDatabase.GetCO2TotalReduced();

            var all = await _userActivityLogDatabase.GetAllActivitiesLogged();
            foreach (var i in all )
            {
                System.Diagnostics.Debug.WriteLine(System.Text.Json.JsonSerializer.Serialize(i));

            }
        }
    }
}