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
                if (Shell.Current != null)
                    await Shell.Current.GoToAsync(nameof(ConsentPage));
            }

            var hasConsent = await _consentDatabase.HasConsent();

            if (hasConsent)
            {
                if (Shell.Current != null)
                    await Shell.Current.GoToAsync(nameof(DashboardPage));
            }
            else
            {
                if (Shell.Current != null)
                    await Shell.Current.GoToAsync(nameof(ConsentPage));
            }
        }
    }
}