using Xunit;
using Moq;
using System.Collections.Generic;
//using Microsoft.Maui.Controls.PlatformConfiguration.Android.Content.PM;
using MauiScreenTime.Services;
using MauiScreenTime.ViewModels;
using MauiScreenTime.Data;


namespace MauiScreenTimeTests
{

    public class MockUsageStatsService : IUsageStatsService
    {
        public Task<List<AppUsageModel>> GetAppUsageAsync()
        {
            return Task.FromResult(new List<AppUsageModel>
        {
            new AppUsageModel
            {
                PackageName = "com.test.app",
                AppName = "Test App",
                UsageTimeMilliseconds = TimeSpan.FromMinutes(30)
            }
        });
        }

        IList<string> IUsageStatsService.GetInstalledPackages()
        {
            var result = new List<string>();
            return (IList<string>)Task.FromResult(result);
        }

        Task<bool> IUsageStatsService.HasPermissionAsync()
        {
            return Task.FromResult(new Boolean());
        }


        [Fact]
        public async Task GetAppUsageAsync_ReturnsMockedData()
        {
            // Arrange
            var service = new MockUsageStatsService();

            // Act
            var result = await service.GetAppUsageAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test App", result[0].AppName);
            Assert.Equal(TimeSpan.FromMinutes(30), result[0].UsageTimeMilliseconds);
        }

        [Fact]
        public void HasPermission_WhenSet_ShouldReturnCorrectValue()
        {
            // Arrange
            var viewModel = new DashboardViewModel(null);

            // Act
            viewModel.hasPermission = true;

            // Assert
            Assert.True(viewModel.hasPermission);
        }
    }
}
//    public class AppUsageViewModelTests
//    {
//        [Fact]
//        public void HasPermission_WhenSet_ShouldReturnCorrectValue()
//        {
//            // Arrange
//            var viewModel = new DashboardViewModel(null); // null because not calling service methods to avoid mocking

//            // Act
//            viewModel.hasPermission = true;

//            // Assert
//            Assert.True(viewModel.hasPermission);
//        }

//            [Fact]
//            public async Task GetAppUsageAsync_ReturnsListOfAppUsageInfo()
//            {
//                // Arrange
//                var service = new UsageStatsService();

//                // Act
//                var result = await service.GetAppUsageAsync();

//                // Assert
//                Assert.NotNull(result);
//                Assert.IsType<List<AppUsageModel>>(result);
//            }

//        [Fact]
//        public async Task GetAppUsageAsync_ReturnsNonNullList()
//        {
//            // Arrange
//            var service = new UsageStatsService();

//            // Act
//            var result = await service.GetAppUsageAsync();

//            // Assert
//            Assert.NotNull(result);
//        }
//    }
//}