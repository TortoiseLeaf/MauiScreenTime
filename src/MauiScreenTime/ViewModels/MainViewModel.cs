using MauiScreenTime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.ViewModels
{
    public class MainViewModel
    {
        private readonly IStartupService _startupService;

        public MainViewModel(IStartupService startupService)
        {
            _startupService = startupService;
            
        }

        public async Task InitializeConsentCheckAsync()
        {

        await _startupService.InitializeConsentCheckAsync();
        
        }
    }
}
