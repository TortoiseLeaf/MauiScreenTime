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

        public async Task InitializeConsentCheckAsync()
        {

        await _startupService.InitializeConsentCheckAsync();
        
        }
        private async Task NavigateToDashboardPage()
        {

            await Shell.Current.GoToAsync(nameof(DashboardPage));

        }
    }
}
