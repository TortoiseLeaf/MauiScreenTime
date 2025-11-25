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

        public async Task InitializeAsync()
        {
            var hasConsent = await _consentDatabase.HasConsent();

            if (_consentDatabase == null)
            {
                await Shell.Current.GoToAsync("//ConsentPage");
            }
            if (hasConsent)
            {
                await Shell.Current.GoToAsync("//DashboardPage");
            }
            else
            {
                await Shell.Current.GoToAsync("//ConsentPage");
            }
        }
    }
}