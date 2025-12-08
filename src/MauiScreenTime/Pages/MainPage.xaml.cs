using MauiScreenTime.Data;
using MauiScreenTime.Pages;
using MauiScreenTime.ViewModels;

namespace MauiScreenTime
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;

            InitConsentCheck();

        }

        private async void InitConsentCheck()
        {
            await _viewModel.InitializeConsentCheckAsync();

        }

    }
}