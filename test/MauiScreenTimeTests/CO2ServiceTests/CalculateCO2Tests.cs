using MauiScreenTime.Data;
using MauiScreenTime.Data.Interfaces;
using MauiScreenTime.Services;
using MauiScreenTime.Services.Interfaces;
using Moq;
using Moq.Language.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTimeTests.CO2ServiceTests
{
    public class CalculateCO2Tests
    {
        private readonly Mock<IConversionTableDatabase> _mockConversionDatabase;
        private readonly Mock<IAppUsageDatabase> _mockAppUsageDatabase;
        //private readonly Mock<ICO2Service> _co2Service;
        private readonly ICO2Service _co2Service;
        private AppUsageModel mockAppData;

        public CalculateCO2Tests()
        {
            _mockConversionDatabase = new Mock<IConversionTableDatabase>();
            _mockAppUsageDatabase = new Mock<IAppUsageDatabase>();
            _co2Service = new CO2Service(_mockConversionDatabase.Object, _mockAppUsageDatabase.Object, null);
        }

        [Fact]
        public async Task CalculateCO2eAsync_WithValidData_CalculatesResult()
        {
            // Arrange
            var testData = new AppUsageModel { PackageName = "Test", UsageTimeMinutes = 10 };
            var conversionTableEntry = new ConversionTableModel
            {
                PackageName = "Test",
                AppName = "Test",
                CO2Mins = 5.0
            };

            _mockConversionDatabase
                .Setup(x => x.GetConversionTableEntryByPackageName("Test"))
                .ReturnsAsync(conversionTableEntry);

            // Act
            var result = await _co2Service.CalculateCO2eAsync(testData);

            // Assert
            Assert.Equal(50.0, result.CO2e);// 5.0 * 10
        }

        [Fact]
        public async Task MethodAsync_WhenObjectNotFound_ReturnsUnmodifiedData()
        {
            // Arrange
            var testData = new AppUsageModel { PackageName = "NotFound", UsageTimeMinutes = 10 };

            _mockConversionDatabase
                .Setup(x => x.GetConversionTableEntryByPackageName("NotFound"))
                .ReturnsAsync((ConversionTableModel)null);

            // Act
            var result = await _co2Service.CalculateCO2eAsync(testData);

            // Assert
            Assert.Equal(0, result.CO2e);
        }

        [Fact]
        public async Task CalculateCO2eAsync_ReturnsAppUsageModelType()
        {
            // Arrange
            var testData = new AppUsageModel { PackageName = "Test", UsageTimeMinutes = 10 };
            var conversionTableEntry = new ConversionTableModel 
            { 
                PackageName = "Test", 
                AppName = "Test", 
                CO2Mins = 0.5 
            };

            _mockConversionDatabase
                .Setup(x => x.GetConversionTableEntryByPackageName("Test"))
                .ReturnsAsync(conversionTableEntry);

            // Act
            var result = await _co2Service.CalculateCO2eAsync(testData);

            // Assert
            Assert.IsType<AppUsageModel>(result);
        }
    }
}
