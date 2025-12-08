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
            _consentDatabase = consentDatabase;
        }
        public async Task InitializeConsentCheckAsync()
        {
            if (_consentDatabase == null)
            {
                    await Shell.Current.GoToAsync(nameof(ConsentPage));
            }

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