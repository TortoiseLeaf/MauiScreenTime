using MauiScreenTime.Pages;
using MauiScreenTime.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiScreenTime.ViewModels
{
    public partial class MainViewModel
    {
        private readonly IStartupService _startupService;

        public MainViewModel(IStartupService startupService)
        {
            _startupService = startupService;
            NavigateToDashboardCommand = new Command(async () => await NavigateToDashboardPage());

        }

        public ICommand NavigateToDashboardCommand { get; }

        // calls the consent check on app startup
        public async Task InitializeConsentCheckAsync()
        {
            try
            {
                await _startupService.InitializeConsentCheckAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initialising consent check in Main viewModel: {ex}");

                await Shell.Current.DisplayAlert("Error", "Unable to check policy consent. Please try again.", "OK");
            }

        }
        private async Task NavigateToDashboardPage()
        {
            // is this necessary or will it just not redirect if it fails, instead of a crash?
            try
            {
                await Shell.Current.GoToAsync(nameof(DashboardPage));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navigating to dashboard from mainpage: {ex}");

                await Shell.Current.DisplayAlert("Error", "Unable to load page. Please try again.", "OK");
            }

        }
    }
}
