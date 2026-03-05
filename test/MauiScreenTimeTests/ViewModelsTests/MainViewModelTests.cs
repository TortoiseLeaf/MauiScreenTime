using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiScreenTime.Services;
using MauiScreenTime.ViewModels;
using MauiScreenTime.Services;

namespace MauiScreenTimeTests.ViewModelsTests
{
    internal class MockStartupService : IStartupService
    {
        public bool WasCalled { get; private set; }
        public bool ShouldThrow { get; set; }

        public async Task InitializeConsentCheckAsync()
        {
            WasCalled = true;
            if (ShouldThrow)
                throw new Exception("Simulated startup failure");
            await Task.CompletedTask;
        }
    }
    public class MainViewModelTests
    {
        private MockStartupService _startupService;
        private MainViewModel _viewModel;

        public MainViewModelTests()
        {
            _startupService = new MockStartupService();
            _viewModel = new MainViewModel(_startupService);
        }

        // test public async Task InitializeConsentCheckAsync()
        [Fact]
        public async Task InitializeConsentCheckAsync_CallsStartupService()
        {
            await _viewModel.InitializeConsentCheckAsync();

            Assert.True(_startupService.WasCalled);
        }

    }
}
