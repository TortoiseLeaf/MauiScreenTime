using CommunityToolkit.Mvvm.ComponentModel;
using MauiScreenTime.Data;
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
        private readonly IDailyCO2Database _dailyCO2Database;  
        private readonly IUserGoalsDatabase _userGoalsDatabase;

        [ObservableProperty]
        private double _totalSaved = new(); //sum of each reduction since start day
        [ObservableProperty]
        private double _progress = new(); //current progress towards next level       
        [ObservableProperty]
        private double _latestDailyCO2 = new(); //last daily total
        [ObservableProperty]
        private double _previousDailyCO2 = new(); //second to last daily total
        [ObservableProperty]
        private double _latestReduction = new(); //difference between last and second to last elements
        
        private const int _levelStep = 200; //grams to save to reach next level, in future this can change based on level

        /// <summary>
        /// these variables are for the progress bar section using the ProgressBar element from .NET MAUI
        /// </summary>
        [ObservableProperty]
        private int _level; //this can be calculated from total saved
        [ObservableProperty]
        private double _progressPercentage; //this is to normalise the bar, ProgressBar wants values between 0.0 and 1.0

        public GoalViewModel(IDailyCO2Database dailyCO2Database, IUserGoalsDatabase userGoalsDatabase)
        {
            _dailyCO2Database = dailyCO2Database;
            _userGoalsDatabase = userGoalsDatabase;
        }

        public async Task GetAllAsync()
        {
            List<DailyCO2Model> allDaily;

/*#if DEBUG
            allDaily = new List<DailyCO2Model>
                {
                    new() { CO2Total = 1000 },
                    new() { CO2Total = 900 },
                    new() { CO2Total = 810 },
                };
#else*/
            allDaily = await _dailyCO2Database.GetAllOrderedByDateAsync();
//#endif
            LatestDailyCO2 = allDaily.Count >= 1 ? allDaily[^1].CO2Total : 0;
            PreviousDailyCO2 = allDaily.Count >= 2 ? allDaily[^2].CO2Total : 0;
            
            if (PreviousDailyCO2 - LatestDailyCO2 > 0)
            {
                LatestReduction = PreviousDailyCO2 - LatestDailyCO2;
            }
            else
            {
                LatestReduction = 0;
            }

            double totalSaved = CalculateTotalSaved(allDaily);
            TotalSaved = totalSaved;
            Progress = totalSaved % _levelStep;
            ProgressPercentage = Progress / _levelStep; //this is needed for the ProgressBar element in the view
            Level = (int)(TotalSaved / _levelStep) + 1;

            await _userGoalsDatabase.SaveTodayGoals((int)Level, TotalSaved, Progress);
        }
        public static double CalculateTotalSaved(IReadOnlyList<DailyCO2Model> days)
        {
            double totalSaved = 0;
            for (int i = 1;  i < days.Count; i++)
            {
                var reduction = days[i - 1].CO2Total - days[i].CO2Total;
                if (reduction > 0) totalSaved += reduction;
            }
            return totalSaved;
        }        
    }
}