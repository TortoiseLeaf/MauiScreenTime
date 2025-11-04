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
            CheckConsent();
        }

        // CALL A FUCNTION THAT IF BOOL HASCONSENT DOES BELOW, ELSE NAVIGATE TO MAINPAGE

        private async void CheckConsent()
        {
           bool hasConsent = await _consentDatabase.HasConsent();

            if (!hasConsent) 
            {
                NavigateToConsentPage();

            }

        }
        private async void NavigateToConsentPage()
        {
            await Shell.Current.GoToAsync(nameof(ConsentPage));
        }
        
    }
}
