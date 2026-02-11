//using Android.Media;
using MauiScreenTime.Data;
using MauiScreenTime.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Services
{

    public class StartupService : IStartupService
    {
        private readonly ConsentDatabase _consentDatabase;
        private bool _hasConsent;

        public StartupService(ConsentDatabase consentDatabase)
        {
            if (consentDatabase != null)
            {
                _consentDatabase = consentDatabase;
            }
            else
            {
                Shell.Current.GoToAsync(nameof(ConsentPage));

            }
        }
        
        // checks for policy consent boolean from dedicated db
        public async Task InitializeConsentCheckAsync()
        {

            _hasConsent = await _consentDatabase.HasConsent();

            if (_hasConsent)
            {
                    await Shell.Current.GoToAsync(nameof(DashboardPage));
            }
            else
            {
                    await Shell.Current.GoToAsync(nameof(ConsentPage));
            }
        }
    }
}