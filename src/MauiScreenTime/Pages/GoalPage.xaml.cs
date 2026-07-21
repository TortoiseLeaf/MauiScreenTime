using MauiScreenTime.ViewModels;
using MauiScreenTime.Pages;

namespace MauiScreenTime.Pages;

public partial class GoalPage : ContentPage
{
    private readonly GoalViewModel _viewModel;

    public GoalPage(GoalViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.GetAllAsync();
    }

}
