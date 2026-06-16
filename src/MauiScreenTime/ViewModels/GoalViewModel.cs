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
 

        public GoalViewModel(ICO2Service co2Service, IDailyCO2Database dailyCO2Database)
        {
            _dailyCO2Database = dailyCO2Database;
        }

        public async Task GetAllAsync()
        {
            List<DailyCO2Model> allDaily = await _dailyCO2Database.GetAllOrderedByDateAsync();

            LatestDailyCO2 = allDaily.Count >= 1 ? allDaily[^1].CO2Total : 0;
            PreviousDailyCO2 = allDaily.Count >= 2 ? allDaily[^2].CO2Total : 0;

            LatestReduction = PreviousDailyCO2 - LatestDailyCO2;

            double totalSaved = CalculateTotalSaved(allDaily);
            TotalSaved = totalSaved;
            Progress = totalSaved % _levelStep;
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