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
using MauiScreenTime.Pages;
using System.Threading.Tasks;

#if ANDROID
using Android.Util;
#endif

namespace MauiScreenTime.ViewModels
{

    public class BarItem
    {
        public double Height { get; set; }
        public string? Label { get; set; }
        public double Value { get; set; }
        public Color? BarColor { get; set; }
    }

    public partial class DashboardViewModel : ObservableObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        private readonly IUsageStatsService _usageStatsService;
        private readonly IUserActivityLogDatabase _userActivityLogDatabase;
        private readonly ICO2Service _co2Service;
        public bool hasPermission;

        [ObservableProperty]
        private List<AppUsageModel> _appUsageList = [];
        [ObservableProperty]
        private ObservableCollection<AppUsageModel> _appUsageListCO2 = [];
        [ObservableProperty]
        private double _co2Total = new();
        [ObservableProperty]
        private double _co2TotalReduced = new();

        [ObservableProperty]
        private double _co2DailySavedDebug = new();
        
        [ObservableProperty]
        private double _co2TotalToday = new();

    

        public ObservableCollection<BarItem> CurrentChart { get; set; }

        // Dynamic Y axis labels
        public ObservableCollection<string> YAxisLabels { get; set; } = [];

        private List<BarItem> ScreenTimeData;
        private List<BarItem> CO2eData;

        // Maximum height of the chart
        const double MaxBarHeight = 185;
        const double ScreenTimeAxisMax = 360; // minutes
        const double CO2AxisMax = 200; // grams

        bool isScreenTimeActive = true;
        bool isFirstLoad = true;

        public string STTotal { get; set; }
        public string CO2eTotal { get; set; }

        // Chart colours
        readonly Color ScreenTimeBarColor = Color.FromArgb("#41b6e6");
        readonly Color CO2eBarColor = Color.FromArgb("#1b365d");

        // Colours for active/inactive states of the toggle buttons
        readonly Color ActiveColor = Color.FromArgb("#7C3AED");
        readonly Color InactiveColor = Color.FromArgb("#CDB9F0");
        readonly Color ActiveTextColor = Colors.White;
        readonly Color InactiveTextColor = Color.FromArgb("#4C1D95");

        private Color _buttonBackgroundColor = Colors.MediumPurple;
        public Color ButtonBackgroundColor
        {
            get => _buttonBackgroundColor;
            set
            {
                _buttonBackgroundColor = value;
                OnPropertyChanged();
            }
        }

        public DashboardViewModel(IUsageStatsService usageStatsService, ICO2Service co2Service, IUserActivityLogDatabase userActivityLogDatabase)
        {

            _usageStatsService = usageStatsService;
            _userActivityLogDatabase = userActivityLogDatabase;
            _co2Service = co2Service;


            CurrentChart = new ObservableCollection<BarItem>(
            Enumerable.Range(0, 10).Select(_ => new BarItem { Height = 0 })
        );

            //DEBUG();
            _ = InitialiseAsync();
            _ = ShowScreenTime();
            
            //GetDataSoFar();

        }

        public async Task DEBUG()
        {
            await _userActivityLogDatabase.DEBUG(30, 0, 0, 0);
            await _userActivityLogDatabase.DEBUG2(110, 0, 0, 0); // 200 TOTAL DAY BEFORE AND 100 TOTAL TODAY
            

        }

        public async Task InitialiseAsync()
        {
            await PopulateAppCO2ListAsync();
            await LoadData();
            await CalculateTotals();
            await ShowScreenTime();
            await ShowCO2e();


            await GetAndStoreCO2Total();
        }

        private async Task PopulateAppCO2ListAsync()
        {
            await GetUsageData();
            await GetCO2Coversion();
            

        }

        // are these working?
        public Func<List<BarItem>, Color, Color, Task> AnimateBarsAsync { get; set; }
        private void UpdateButton(bool isSelected)
        {
            ButtonBackgroundColor = isSelected ? ActiveColor : Colors.MediumPurple;
        }


        async Task LoadData()
        {
            
            ScreenTimeData = [.. AppUsageListCO2.Select(obj => new BarItem
            {
                Label = obj.AppName.Length >= 3 ? obj.AppName[..3] : obj.AppName,
                Value = obj.UsageTimeMinutes
            })];

            CO2eData = [.. AppUsageListCO2.Select(obj => new BarItem
            {
                Label = obj.AppName.Length >= 3 ? obj.AppName[..3] : obj.AppName,

                Value = obj.CO2e
            })];


            await CalculateTotals();
        }

        // Scales raw data to visual bar heights - keeps the chart proportional regardless of values
        List<BarItem> ScaleData(List<BarItem> source, Color color, double axisMax)
        {
            return source.Select(item => new BarItem
            {
                Height = Math.Min(item.Value / axisMax, 1) * MaxBarHeight,
                Label = item.Label,
                Value = item.Value,
                BarColor = color
            }).ToList();
        }

        async Task CalculateTotals()
        {
            //Screen Time total
            double totalMinutes = ScreenTimeData.Sum(value => value.Value);

            int hours = (int)(totalMinutes / 60);
            int minutes = (int)(totalMinutes % 60);
            STTotal = $"Total Screen Time: {hours}h {minutes}m";

            // CO2e total
            double totalCO2e = CO2eData.Sum(value => value.Value);
            CO2eTotal = $"Total CO₂e: {totalCO2e}g";

            OnPropertyChanged(nameof(STTotal));
            OnPropertyChanged(nameof(CO2eTotal));

        }

        // Updates the toggle button colours, depending on which one is active
        //private void UpdateToggleButton(bool isScreenTimeSelected)
        //{
        //    if (isScreenTimeSelected)
        //    {
        //        ScreenTimeButton.BackgroundColor = ActiveColor;
        //        ScreenTimeButton.TextColor = ActiveTextColor;
        //        CO2eButton.BackgroundColor = InactiveColor;
        //        CO2eButton.TextColor = InactiveTextColor;
        //    }
        //    else
        //    {
        //        ScreenTimeButton.BackgroundColor = InactiveColor;
        //        ScreenTimeButton.TextColor = InactiveTextColor;
        //        CO2eButton.BackgroundColor = ActiveColor;
        //        CO2eButton.TextColor = ActiveTextColor;
        //    }

        //    ScreenTimeButton.IsEnabled = !isScreenTimeActive;
        //    CO2eButton.IsEnabled = isScreenTimeActive;
        //}


        // Linearly interpolates between two colours
        

        public List<BarItem> CurrentBarData { get; private set; }

        //public async Task LoadBarDataAsync(...)
        //{
        //    CurrentBarData = /* your data logic */;
        //    OnPropertyChanged(nameof(CurrentBarData));
        //}

        // Animates bar height and colour simultaneously
        //async Task AnimateBars(List<BarItem> newData, Color fromColor, Color toColor)
        //{
        //    // Direct references to each bar in XAML
        //    var bars = new[]
        //    {
        //    Bar0, Bar1, Bar2, Bar3, Bar4, Bar5, Bar6, Bar7, Bar8, Bar9
        //};

        //    // Prevent overflow if data count changes
        //    int count = Math.Min(bars.Length, newData.Count);

        //    for (int index = 0; index < count; index++)
        //    {
        //        await Task.Delay(20);   // Add delay to each bar

        //        int control = index;

        //        // First load starts bars from zero height
        //        double start = isFirstLoad ? 0 : bars[control].HeightRequest;

        //        double end = newData[control].Height; // Target height

        //        var animation = new Animation(aniProgress =>
        //        {
        //            // Height animation
        //            bars[control].HeightRequest = start + (end - start) * aniProgress;

        //            // Colour fade animation
        //            bars[control].BackgroundColor = ColourFade(fromColor, toColor, aniProgress);
        //        }, 0, 1);

        //        animation.Commit(this, $"BarAnim{index}", 16u, (uint)(500 + index * 40), Easing.CubicInOut);
        //    }

        //    // First load animation completed
        //    isFirstLoad = false;
        //}


        // Switch chart to Screen Time
        async Task ShowScreenTime()
        {
            isScreenTimeActive = true;

            UpdateYAxis();

            try {
                
                    var scaled = ScaleData(ScreenTimeData, ScreenTimeBarColor, ScreenTimeAxisMax);

                    // Animate from current state ? new state
                    //await AnimateBarsAsync(scaled, isFirstLoad ? ScreenTimeBarColor : CO2eBarColor, ScreenTimeBarColor);

                    // Update binding source after animation
                    CurrentChart = new ObservableCollection<BarItem>(scaled);
                    OnPropertyChanged(nameof(CurrentChart));

                    UpdateButton(true);
                
            } catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine($"Error getting usage data, possibly none: {ex}");

            }
        }


        // Switch chart to CO?e
        async Task ShowCO2e()
        {
            isScreenTimeActive = false;

            UpdateYAxis();

            try { 
                var scaled = ScaleData(CO2eData, CO2eBarColor, CO2AxisMax);

                // Animate from current state ? new state
                //await AnimateBarsAsync(scaled, ScreenTimeBarColor, CO2eBarColor);

                // Update binding source after animation
                CurrentChart = new ObservableCollection<BarItem>(scaled);
                OnPropertyChanged(nameof(CurrentChart));

                UpdateButton(false);
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting usage data, possibly none: {ex}");

            }

        }

        // Alternate Y axis values depending on which chart is displayed
        void UpdateYAxis()
        {
            YAxisLabels.Clear();

            if (isScreenTimeActive)
            {
                YAxisLabels.Add("6h");
                YAxisLabels.Add("4h");
                YAxisLabels.Add("2h");
                YAxisLabels.Add("1h");
                YAxisLabels.Add("0h");
            }
            else
            {
                YAxisLabels.Add("200g");
                YAxisLabels.Add("150g");
                YAxisLabels.Add("100g");
                YAxisLabels.Add("50g");
                YAxisLabels.Add("0g");
            }
        }

        public async void OnScreenTimeClicked() => await ShowScreenTime();
        public async void OnCO2eClicked() => await ShowCO2e();
        public async void OnGoalClicked() => await Shell.Current.GoToAsync(nameof(GoalPage));

        

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
                        AppUsageList.Clear();
                        foreach (var app in usageData)
                        {
                            AppUsageList.Add(app);
                        }
                    } 
                    else
                    {
                        AppUsageList =
                        [
                            new() { UsageTimeMinutes = 0 },
                        ];
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

            foreach (var app in AppUsageList)
            {
                // try/catch
                var appData = await _co2Service.CalculateCO2eAsync(app);

                AppUsageListCO2.Add(appData);

            }
        }

        public async Task GetAndStoreCO2Total()
        {

            try
            {
                Co2Total = await _co2Service.CalculateCO2TotalAsync(AppUsageList);
                await _userActivityLogDatabase.AddActivityLog(Co2Total, 0, 0, 0);

                var TodayTotalCO2Obj = await _userActivityLogDatabase.GetHighestCO2DailyTotalByDate(DateTime.Now);

            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error totalling CO2e in dashboard: {ex}");

                await Shell.Current.DisplayAlert("Error", "Unable to total CO2e.", "OK");
            }


        }


    }
}