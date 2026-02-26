using MauiScreenTime.Data;
using MauiScreenTime.Data.Interfaces;
using MauiScreenTime.Services;
using MauiScreenTime.Services.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTimeTests.CO2ServiceTests
{
    public class CalculateTotalCO2Tests
    {
        private readonly Mock<IConversionTableDatabase> _mockConversionDatabase;
        private readonly Mock<IAppUsageDatabase> _mockAppUsageDatabase;
        private readonly ICO2Service _co2Service;
        private AppUsageModel mockAppData;

        public CalculateTotalCO2Tests()
        {
            _mockConversionDatabase = new Mock<IConversionTableDatabase>();
            _mockAppUsageDatabase = new Mock<IAppUsageDatabase>();
            _co2Service = new CO2Service(_mockConversionDatabase.Object, _mockAppUsageDatabase.Object, null);
        }

        [Fact]
        public async Task CalculateCO2TotalAsync_WithValidList_ReturnsSumOfMins()
        {
            // Arrange
            var mockList = new List<AppUsageModel>
    {
        new AppUsageModel { PackageName = "Item1", UsageTimeMinutes = 5 },
        new AppUsageModel { PackageName = "Item2", UsageTimeMinutes = 10 },
        new AppUsageModel { PackageName = "Item3", UsageTimeMinutes = 15 }
    };

            _mockConversionDatabase.Setup(x => x.GetMatchingCO2Mins("Item1")).ReturnsAsync(3.0);
            _mockConversionDatabase.Setup(x => x.GetMatchingCO2Mins("Item2")).ReturnsAsync(2.0);
            _mockConversionDatabase.Setup(x => x.GetMatchingCO2Mins("Item3")).ReturnsAsync(1.0);

            // Act
            var result = await _co2Service.CalculateCO2TotalAsync(mockList);

            // Assert
            Assert.Equal(50.0, result); // 15 + 20 + 15
        }

        [Fact]
        public async Task CalculateCO2TotalAsync_WithNullList_ReturnsZero()
        {
            // Arrange
            List<AppUsageModel> mockList = null;

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
    }
}
