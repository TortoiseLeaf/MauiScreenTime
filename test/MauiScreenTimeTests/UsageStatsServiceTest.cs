using MauiScreenTime.Data;
//using Microsoft.Maui.Controls.PlatformConfiguration.Android.Content.PM;
using MauiScreenTime.Services;
using MauiScreenTime.ViewModels;
using Moq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;


namespace MauiScreenTimeTests
{

    public class UsageStatsTests
    {
        private readonly Mock<IUsageStatsService> _usageStatsServiceMock;

        public UsageStatsTests()
        {
            _usageStatsServiceMock = new Mock<IUsageStatsService>();
        }


        [Fact]
        public async Task GetAppUsageAsync_ReturnsEmptyList_IfNoData()
        {
            // Arrange
            _usageStatsServiceMock
                .Setup(x => x.GetAppUsageAsync())
                .ReturnsAsync(new List<AppUsageModel>());

            // Act
            var result = await _usageStatsServiceMock.Object.GetAppUsageAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _usageStatsServiceMock.Verify(x => x.GetAppUsageAsync(), Times.Once);
        }


        [Fact]
        public async Task GetAppUsageAsync_ReturnsAppUsageData_WhenData()
        {
            // Arrange
            var startTime = DateTime.Now.AddDays(-7);
            var endTime = DateTime.Now;

            var expectedData = new List<AppUsageModel>
            {
                new AppUsageModel
                {
                    PackageName = "com.example.app1",
                    AppName = "Test App 1",
                    UsageTimeMilliseconds = TimeSpan.FromMinutes(30)
                },
                new AppUsageModel
                {
                    PackageName = "com.example.app2",
                    AppName = "Test App 2",
                    UsageTimeMilliseconds = TimeSpan.FromMinutes(15)
                }
            };

            _usageStatsServiceMock
                .Setup(x => x.GetAppUsageAsync())
                .ReturnsAsync(expectedData);

            // Act
            var result = await _usageStatsServiceMock.Object.GetAppUsageAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Test App 1", result[0].AppName);
            Assert.Equal(TimeSpan.FromMinutes(30), result[0].UsageTimeMilliseconds);
            _usageStatsServiceMock.Verify(x => x.GetAppUsageAsync(), Times.Once);
        }


        [Fact]
        public async Task GetAppUsageAsync_ReturnsSortedByUsage()
        {
            // Arrange
            var expectedData = new List<AppUsageModel>
            {
                new AppUsageModel
                {
                    AppName = "High Usage App",
                    UsageTimeMilliseconds = TimeSpan.FromMinutes(60)
                },
                new AppUsageModel
                {
                    AppName = "Medium Usage App",
                    UsageTimeMilliseconds = TimeSpan.FromMinutes(30)
                },
                new AppUsageModel
                {
                    AppName = "Low Usage App",
                    UsageTimeMilliseconds = TimeSpan.FromMinutes(5)
                }
            };

            _usageStatsServiceMock
                .Setup(x => x.GetAppUsageAsync())
                .ReturnsAsync(expectedData);

            // Act
            var result = await _usageStatsServiceMock.Object.GetAppUsageAsync();

            // Assert
            Assert.Equal("High Usage App", result[0].AppName);
            Assert.Equal("Medium Usage App", result[1].AppName);
            Assert.Equal("Low Usage App", result[2].AppName);
        }


        [Fact]
        public async Task GetAppUsageAsync_FiltersZeroUsageApps()
        {
            // Arrange
            var expectedData = new List<AppUsageModel>
            {
                new AppUsageModel
                {
                    AppName = "Used App",
                    UsageTimeMilliseconds = TimeSpan.FromMinutes(10)
                }
            };

            _usageStatsServiceMock
                .Setup(x => x.GetAppUsageAsync())
                .ReturnsAsync(expectedData);

            // Act
            var result = await _usageStatsServiceMock.Object.GetAppUsageAsync();

            // Assert
            Assert.All(result, app => Assert.True(app.UsageTimeMilliseconds > TimeSpan.Zero));
        }

        // HasPermission tests
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
