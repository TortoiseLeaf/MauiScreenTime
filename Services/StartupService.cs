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

        public async Task InitializeConsentCheckAsync()
        {
            if (_consentDatabase == null)
            {
                await Shell.Current.GoToAsync(nameof(ConsentPage));
            }

            var hasConsent = await _consentDatabase.HasConsent();

            if (hasConsent)
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