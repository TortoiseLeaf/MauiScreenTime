using MauiScreenTime.Data.Interfaces;
using MauiScreenTime.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiScreenTime.Data;
using MauiScreenTime.Services.Interfaces;

namespace MauiScreenTimeTests.ViewModelsTests
{
    //This class implements IConsentDatabase, it emulates the db by sroting consent info in a list
    public class MockConsentDatabase : IConsentDatabase
    {
        private List<UserConsentModel> _consentHistory = new List<UserConsentModel>();

        public Task<bool> HasConsent()
        {
            var latestConsent = _consentHistory
                 .OrderByDescending(c => c.GrantedAt)
                 .FirstOrDefault();

            return Task.FromResult(latestConsent?.IsGranted ?? false);
        }
        public Task GrantConsent(string version = "1.0")
        {
            _consentHistory.Add(new UserConsentModel
            {
                IsGranted = true,
                GrantedAt = DateTime.UtcNow,
                Version = version
            });

            return Task.CompletedTask;
        }
        public Task RevokeConsent(string version = "1.0")
        {
            _consentHistory.Add(new UserConsentModel
            {
                IsGranted = false,
                RevokedAt = DateTime.UtcNow,
                Version = version
            });

            return Task.CompletedTask;
        }
        public Task<List<UserConsentModel>> GetConsentHistory()
        {
            return Task.FromResult(new List<UserConsentModel>(_consentHistory));
        }
        public Task DeleteAllConsents()
        {
            _consentHistory.Clear();
            return Task.CompletedTask;
        }
    }

    //This class mocks the UsageStatsService, also needed to instantiate a consent view model
    public class MockUsageStatsService : IUsageStatsService
    {
        public bool ShouldHavePermission { get; set; } = true;
        public List<string> InstalledPackages { get; set; } = new List<string>();
        public List<AppUsageModel> AppUsageData { get; set; } = new List<AppUsageModel>();

        public Task<bool> HasPermissionAsync()
        {
            return Task.FromResult(ShouldHavePermission);
        }

        public IList<string> GetInstalledPackages()
        {
            return InstalledPackages;
        }
        

        public Task<List<AppUsageModel>> GetAppUsageAsync() 
        {
            return Task.FromResult(AppUsageData);
        }
    }
    public class ConsentViewModelTests
    {

        //public testable attributes:
        //HasConsent
        //public ICommand GrantConsentCommand
        //public ICommand RevokeConsentCommand
        //public ICommand DeleteAllCommand


        //test: HasConsentReturnsABool
        [Fact]
        public async Task HasConsentReturnsABool()
        {
            //Arrange
            //instantiate needed dependencies
            var mockDb = new MockConsentDatabase();
            var mockUsageStats = new MockUsageStatsService
            {
                ShouldHavePermission = true,
                InstalledPackages = new List<string>() { "com.example.app1", "com.example.app2" }
            };
            //instantiate vm
            var consentViewModel = new ConsentViewModel(mockDb, mockUsageStats);
            //Act
            //call HasConsent into a consent bool variable
            bool consent = consentViewModel.HasConsent;
            //Assert
            //assert consent is either true or false
            Assert.IsType<bool>(consent);
        }

        //test: GrantConsentCommandSetsConsentToTrue
        [Fact]
        public async Task GrantConsentCommandSetsConsentToTrue()
        {
            //Arrange
            //instantiate needed dependencies
            var mockDb = new MockConsentDatabase();
            var mockUsageStats = new MockUsageStatsService
            {
                ShouldHavePermission = true,
                InstalledPackages = new List<string>() { "com.example.app1", "com.example.app2" }
            };
            //instantiate vm
            var consentViewModel = new ConsentViewModel(mockDb, mockUsageStats);
            //Act
            //call GrantConsentCommand
            consentViewModel.GrantConsentCommand.Execute(null);
            //Assert
            //assert HasConsent is equal to true
            Assert.True(consentViewModel.HasConsent);
        }

        //test: RevokeConsentCommandRemovesConsent
        [Fact]
        public async Task RevokeConsentCommandRemovesConsent()
        {

            //Arrange
            //instantiate needed dependencies
            var mockDb = new MockConsentDatabase();
            var mockUsageStats = new MockUsageStatsService
            {
                ShouldHavePermission = true,
                InstalledPackages = new List<string>() { "com.example.app1", "com.example.app2" }
            };
            //instantiate vm
            var consentViewModel = new ConsentViewModel(mockDb, mockUsageStats);
            //Act
            //call GrantConsentCommand to make sure HasConsent is set to true
            consentViewModel.GrantConsentCommand.Execute(null);
            Assert.True(consentViewModel.HasConsent);
            //call RevokeConsentCommand
            consentViewModel.RevokeConsentCommand.Execute(null);
            //Assert
            //assert HasConsent is equal to false
            Assert.False(consentViewModel.HasConsent);
        }

        //test: DeleteAllCommandRemovesAllConsents
        [Fact]
        public async Task DeleteAllCommandRemovesAllConsents()
        {

            //Arrange
            //instantiate needed dependencies
            var mockDb = new MockConsentDatabase();
            var mockUsageStats = new MockUsageStatsService
            {
                ShouldHavePermission = true,
                InstalledPackages = new List<string>() { "com.example.app1", "com.example.app2" }
            };
            //instantiate vm
            var consentViewModel = new ConsentViewModel(mockDb, mockUsageStats);
            //Act
            //call GrantConsentCommand to make sure HasConsent is set to true
            consentViewModel.GrantConsentCommand.Execute(null);
            //call DeleteAllCommand
            consentViewModel.DeleteAllCommand.Execute(null);
            //Assert
            //assert HasConsent is equal to false
            Assert.False(consentViewModel.HasConsent);            
        }

        //This are the only public testable methods and attributes. In order to test this the ConsentViewModel will need
        //to be refactored so that it does not directly depend on an instance of the sqlite db.
        //By adding an abstraction (via an interface) for the db we can use dependency injection to mock a db class that
        //would return the expected values when specific methods are called

    }



}
