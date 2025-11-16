using MauiScreenTime.Pages;
using MauiScreenTime.Data;

namespace MauiScreenTime
{
    public partial class MainPage : ContentPage
    {
        private readonly ConsentDatabase _consentDatabase;

        public MainPage(ConsentDatabase consentDatabase)
        {
            InitializeComponent();
            _consentDatabase = consentDatabase;
            CheckDbExists();
            CheckConsent();
        }

        private void CheckDbExists()
        {
            if (_consentDatabase == null)
            {
                NavigateToConsentPage();

            }
        }
        private async void CheckConsent()
        {
            //if (_consentDatabase == null)
            //{
            //    NavigateToConsentPage();

            //}
            bool hasConsent = await _consentDatabase.HasConsent();

            if (hasConsent)
            {
                NavigateToDashboardPage();
            }
            
            else if (!hasConsent) 
            {
                NavigateToConsentPage();

            }

        }
        private async void NavigateToConsentPage()
        {
            await Shell.Current.GoToAsync(nameof(ConsentPage));
        }
        private async void NavigateToDashboardPage()
        {
            await Shell.Current.GoToAsync(nameof(DashboardPage));
        }

    }
}
