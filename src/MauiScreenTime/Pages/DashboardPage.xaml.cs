using MauiScreenTime.ViewModels;
using Microsoft.Maui.Graphics;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MauiScreenTime.Pages;

public class BarItem
{
    public double Height { get; set; }
    public string Label { get; set; }
    public double Value { get; set; }
    public Color BarColor { get; set; }
}

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();

        // Set this page as the binding source for XAML
        BindingContext = this;

        LoadData();

        // Create 10 placeholder bars at height 0
        CurrentChart = new ObservableCollection<BarItem>(Enumerable.Range(0, 10).Select(_ => new BarItem { Height = 0 }));

        _ = ShowScreenTime();
    }

    public ObservableCollection<BarItem> CurrentChart { get; set; }

    // Dynamic Y axis labels
    public ObservableCollection<string> YAxisLabels { get; set; } = new();

    private List<BarItem> ScreenTimeData;
    private List<BarItem> CO2eData;

    // Maximum height of the chart
    const double MaxBarHeight = 185;
    const double ScreenTimeAxisMax = 360; // minutes
    const double CO2AxisMax = 200; // grams

    bool isScreenTimeActive = true;


    // --------------------------------------------------------------------------
    // DATA
    // --------------------------------------------------------------------------

    // Load raw datasets (placeholder data)
    void LoadData()
    {
        ScreenTimeData = new List<BarItem>
        {
            new() { Label = "YT", Value = 360 },
            new() { Label = "Tw", Value = 300 },
            new() { Label = "X", Value = 100 },
            new() { Label = "LI", Value = 70 },
            new() { Label = "Fb", Value = 200 },
            new() { Label = "Sn", Value = 90 },
            new() { Label = "In", Value = 175 },
            new() { Label = "Pin", Value = 60 },
            new() { Label = "Re", Value = 250 },
            new() { Label = "Tik", Value = 300 }
        };

        CO2eData = new List<BarItem>
        {
            new() { Label = "YT", Value = 100 },
            new() { Label = "Tw", Value = 30 },
            new() { Label = "X", Value = 175 },
            new() { Label = "LI", Value = 90 },
            new() { Label = "Fb", Value = 150 },
            new() { Label = "Sn", Value = 200 },
            new() { Label = "In", Value = 90 },
            new() { Label = "Pin", Value = 75 },
            new() { Label = "Re", Value = 130 },
            new() { Label = "Tik", Value = 125 }
        };
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

    // --------------------------------------------------------------------------
    // COLOURS AND ANIMATIONS
    // --------------------------------------------------------------------------

    // Chart colours
    readonly Color ScreenTimeBarColor = Color.FromArgb("#22C55E");
    readonly Color CO2eBarColor = Colors.Orange;

    // Colours for active/inactive states of the toggle buttons
    readonly Color ActiveColor = Color.FromArgb("#7C3AED");
    readonly Color InactiveColor = Color.FromArgb("#CDB9F0");
    readonly Color ActiveTextColor = Colors.White;
    readonly Color InactiveTextColor = Color.FromArgb("#4C1D95");

    // Updates the toggle button colours, depending on which one is active
    private void UpdateToggleButton(bool isScreenTimeSelected)
    {
        if (isScreenTimeSelected)
        {
            ScreenTimeButton.BackgroundColor = ActiveColor;
            ScreenTimeButton.TextColor = ActiveTextColor;
            CO2eButton.BackgroundColor = InactiveColor;
            CO2eButton.TextColor = InactiveTextColor;
        }
        else
        {
            ScreenTimeButton.BackgroundColor = InactiveColor;
            ScreenTimeButton.TextColor = InactiveTextColor;
            CO2eButton.BackgroundColor = ActiveColor;
            CO2eButton.TextColor = ActiveTextColor;
        }

        ScreenTimeButton.IsEnabled = !isScreenTimeActive;
        CO2eButton.IsEnabled = isScreenTimeActive;
    }

    // Linearly interpolates between two colours
    Color ColourFade(Color from, Color to, double pointer)
    {
        float t = (float)pointer;

        return new Color(
            from.Red + (to.Red - from.Red) * t,
            from.Green + (to.Green - from.Green) * t,
            from.Blue + (to.Blue - from.Blue) * t,
            from.Alpha + (to.Alpha - from.Alpha) * t
        );
    }

    // Animates bar height and colour simultaneously
    async Task AnimateBars(List<BarItem> newData, Color fromColor, Color toColor)
    {
        // Direct references to each bar in XAML
        var bars = new[]
        {
            Bar0, Bar1, Bar2, Bar3, Bar4, Bar5, Bar6, Bar7, Bar8, Bar9
        };

        // Prevent overflow if data count changes
        int count = Math.Min(bars.Length, newData.Count);

        for (int index = 0; index < count; index++)
        {
            int control = index;

            double start = bars[control].HeightRequest; // Current height
            double end = newData[control].Height; // Target height

            var animation = new Animation(aniProgress =>
            {
                // Height animation
                bars[control].HeightRequest = start + (end - start) * aniProgress;

                // Colour fade animation
                bars[control].BackgroundColor = ColourFade(fromColor, toColor, aniProgress);
            }, 0, 1);

            animation.Commit(this, $"BarAnim{index}", 16, 400, Easing.CubicInOut);
        }
    }

    // --------------------------------------------------------------------------
    // CHART SWITCHING
    // --------------------------------------------------------------------------

    // Switch chart to Screen Time
    async Task ShowScreenTime()
    {
        isScreenTimeActive = true;

        UpdateYAxis();

        var scaled = ScaleData(ScreenTimeData, ScreenTimeBarColor, ScreenTimeAxisMax);

        // Animate from current state ? new state
        await AnimateBars(scaled, CO2eBarColor, ScreenTimeBarColor);

        // Update binding source after animation
        CurrentChart = new ObservableCollection<BarItem>(scaled);
        OnPropertyChanged(nameof(CurrentChart));

        UpdateToggleButton(true);
    }

    // Switch chart to CO?e
    async Task ShowCO2e()
    {
        isScreenTimeActive = false;

        UpdateYAxis();

        var scaled = ScaleData(CO2eData, CO2eBarColor, CO2AxisMax);

        // Animate from current state ? new state
        await AnimateBars(scaled, ScreenTimeBarColor, CO2eBarColor);

        // Update binding source after animation
        CurrentChart = new ObservableCollection<BarItem>(scaled);
        OnPropertyChanged(nameof(CurrentChart));

        UpdateToggleButton(false);
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

    // --------------------------------------------------------------------------
    // EVENTS
    // --------------------------------------------------------------------------

    // Event handlers have to be async void
    async void OnScreenTimeClicked(object sender, EventArgs e) => await ShowScreenTime();
    async void OnCO2eClicked(object sender, EventArgs e) => await ShowCO2e();
}