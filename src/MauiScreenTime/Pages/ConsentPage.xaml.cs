using MauiScreenTime.ViewModels;

namespace MauiScreenTime.Pages;

public partial class ConsentPage : ContentPage
{
	public ConsentPage(ConsentViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}