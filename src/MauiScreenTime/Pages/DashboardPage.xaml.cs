using MauiScreenTime.Helpers;
using MauiScreenTime.ViewModels;
using Microsoft.Maui.Graphics;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MauiScreenTime.Pages;


public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _viewModel;
    bool isFirstLoad = true;

    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        
    }


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
    //private async void OnSomeEvent(object sender, EventArgs e)
    //{
    //    await _viewModel.LoadBarDataAsync(...);
    //    await AnimateBars(_viewModel.CurrentBarData, fromColor, toColor);
    //}
    private async Task AnimateBars(List<BarItem> newData, Color fromColor, Color toColor)
    {
        var bars = new[] { Bar0, Bar1, Bar2, Bar3, Bar4, Bar5, Bar6, Bar7, Bar8, Bar9 };

        int count = Math.Min(bars.Length, newData.Count);

        for (int index = 0; index < count; index++)
        {
            await Task.Delay(20);

            int control = index;
            double start = isFirstLoad ? 0 : bars[control].HeightRequest;
            double end = newData[control].Height;

            var animation = new Animation(aniProgress =>
            {
                bars[control].HeightRequest = start + (end - start) * aniProgress;
                bars[control].BackgroundColor = ColourFade(fromColor, toColor, aniProgress);
            }, 0, 1);

            animation.Commit(this, $"BarAnim{index}", 16u, (uint)(500 + index * 40), Easing.CubicInOut);
        }

        isFirstLoad = false;
    }

    private void OnScreenTimeClicked(object sender, EventArgs e)
    {
        _viewModel.OnScreenTimeClicked();
    }
    private void OnCO2eClicked(object sender, EventArgs e)
    {
        _viewModel.OnCO2eClicked();
    }
    private void OnGoalClicked(object sender, EventArgs e)
    {
        _viewModel.OnGoalClicked();
    }
}
   