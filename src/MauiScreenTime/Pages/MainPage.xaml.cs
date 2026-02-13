using MauiScreenTime.Data;
using MauiScreenTime.Pages;
using MauiScreenTime.ViewModels;
using System.Windows.Input;

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

        // initialises the policy consent check
        private async void InitConsentCheck()
        {
            try
            {
                await _viewModel.InitializeConsentCheckAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calling consent check in MainPage.xaml.cs: {ex}");
                await Shell.Current.DisplayAlert("Error", "Please try restarting the application", "OK");
            }
        }


    }
}