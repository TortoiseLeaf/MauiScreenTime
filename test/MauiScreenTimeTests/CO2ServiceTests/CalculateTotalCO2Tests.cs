using MauiScreenTime.Data;
using MauiScreenTime.Data.Interfaces;
using MauiScreenTime.Services;
using MauiScreenTime.Services.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/*namespace MauiScreenTimeTests.CO2ServiceTests
{
    public class CalculateTotalCO2Tests
    {
        private readonly Mock<IConversionTableDatabase> _mockConversionDatabase;
        private readonly Mock<IAppUsageDatabase> _mockAppUsageDatabase;
        private readonly Mock<IUserActivityLogDatabase> _mockUserActivityLogDatabase;
        private readonly ICO2Service _co2Service;

        public CalculateTotalCO2Tests()
        {
            _mockConversionDatabase = new Mock<IConversionTableDatabase>();
            _mockAppUsageDatabase = new Mock<IAppUsageDatabase>();
            _mockUserActivityLogDatabase = new Mock<IUserActivityLogDatabase>();
            _co2Service = new CO2Service(_mockConversionDatabase.Object, _mockAppUsageDatabase.Object, _mockUserActivityLogDatabase.Object);
        }

        [Fact]
        public async Task CalculateCO2TotalAsync_WithValidList_ReturnsSumOfMins()
        {
            // Arrange
            var mockList = new List<AppUsageModel>
    {
        new() { PackageName = "Item1", UsageTimeMinutes = 5 },
        new() { PackageName = "Item2", UsageTimeMinutes = 10 },
        new() { PackageName = "Item3", UsageTimeMinutes = 15 }
    };

            _mockConversionDatabase
                   .Setup(x => x.GetConversionTableEntryByPackageName("Item1"))
                   .ReturnsAsync(new ConversionTableModel { PackageName = "Item1", CO2Mins = 3.0 });
            _mockConversionDatabase
                .Setup(x => x.GetConversionTableEntryByPackageName("Item2"))
                .ReturnsAsync(new ConversionTableModel { PackageName = "Item2", CO2Mins = 2.0 });
            _mockConversionDatabase
                .Setup(x => x.GetConversionTableEntryByPackageName("Item3"))
                .ReturnsAsync(new ConversionTableModel { PackageName = "Item3", CO2Mins = 1.0 });

            // Act
            var result = await _co2Service.CalculateCO2TotalAsync(mockList);

            // Assert
            Assert.Equal(50.0, result); // 15 + 20 + 15
        }

        [Fact]
        public async Task CalculateCO2TotalAsync_WithNullList_ReturnsZero()
        {
            // Arrange
            List<AppUsageModel>? mockList = null;

            // Act
            var result = await _co2Service.CalculateCO2TotalAsync(mockList);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task CalculateCO2TotalAsync_WithEmptyList_ReturnsZero()
        {
            // Arrange
            var mockList = new List<AppUsageModel>();

            // Act
            var result = await _co2Service.CalculateCO2TotalAsync(mockList);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task CalculateTotalAsync_CallsAddLog()
        {
            // Arrange
            var mockList = new List<AppUsageModel>
    {
        new AppUsageModel { PackageName = "Item1", UsageTimeMinutes = 5 },
        new AppUsageModel { PackageName = "Item2", UsageTimeMinutes = 10 },
        new AppUsageModel { PackageName = "Item3", UsageTimeMinutes = 15 }
    };

            _mockUserActivityLogDatabase.Setup(x => x.AddActivityLog(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.CompletedTask);
            _mockConversionDatabase.Setup(x => x.GetConversionTableEntryByPackageName(It.IsAny<string>())).ReturnsAsync(new ConversionTableModel { CO2Mins = 5.0 });
            // Act
            await _co2Service.CalculateCO2TotalAsync(mockList);

            // Assert
            _mockUserActivityLogDatabase.Verify(x => x.AddActivityLog(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(0));
        }
    }
}
*/