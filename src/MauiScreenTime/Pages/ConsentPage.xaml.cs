using MauiScreenTime.ViewModels;

namespace MauiScreenTime.Pages;

public partial class ConsentPage : ContentPage
{
	public ConsentPage(ConsentViewModel viewModel)
	{
		InitializeComponent();
		// inject the bindings between viewmodel and view as a dependency (probably repeat this pattern for all viewmodels)
        BindingContext = viewModel;
    }
}