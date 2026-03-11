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
        private int _treesTotal = new();

        public GoalViewModel(ICO2Service co2Service, IUserActivityLogDatabase userActivityLogDatabase)
        {

            _userActivityLogDatabase = userActivityLogDatabase;
            _co2Service = co2Service;

            CalculateDifference();
            IncrementTree();
            GetTreesPlanted();

        }


     public async Task CalculateDifference()
        {
            Co2TotalReduced = await _co2Service.CalculateCO2DifferenceAsync();

        }
     public async Task IncrementTree()
        {
            if (Co2TotalReduced >= 200)
            {
                await _userActivityLogDatabase.AddActivityLog(0, 0, 1);
            }

        }
     public async Task GetTreesPlanted()
        {
            
            TreesTotal = await _userActivityLogDatabase.GetLatestTreesByDate(DateTime.Now);
            
        }
    }
}