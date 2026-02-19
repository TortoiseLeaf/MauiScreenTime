using MauiScreenTime.Data;
using MauiScreenTime.Data.Interfaces;
using MauiScreenTime.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTimeTests
{
    public class CO2ServiceTests
    {
        private readonly Mock<IConversionTableDatabase> _mockConversionDatabase;
        private readonly Mock<IAppUsageDatabase> _mockAppUsageDatabase;
        //private readonly Mock<ICO2Service> _co2Service;
        private readonly ICO2Service _co2Service;
        private AppUsageModel mockAppData;

        public CO2ServiceTests()
        {
            _mockConversionDatabase = new Mock<IConversionTableDatabase>();
            _mockAppUsageDatabase = new Mock<IAppUsageDatabase>();
            _co2Service = new CO2Service(_mockConversionDatabase.Object, _mockAppUsageDatabase.Object);
        }

        [Fact]
        public async Task CalculateCO2eAsync_WithValidData_CalculatesResult()
        {
            // Arrange
            var testData = new AppUsageModel { PackageName = "Test", UsageTimeMinutes = 10 };
            var mockTable = new List<ConversionTableModel>
            {
                new() { PackageName = "Test", CO2Mins = 5.0 }
            };
            

            _mockConversionDatabase.Setup(x => x.GetConversionTable()).ReturnsAsync(mockTable);

            // Act
            var result = await _co2Service.CalculateCO2eAsync(testData);

            // Assert
            Assert.Equal(50.0, result.CO2e); // 5.0 * 10
        }

    }
}
