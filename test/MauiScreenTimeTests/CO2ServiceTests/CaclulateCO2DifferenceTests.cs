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
    public class CalculateCO2DifferenceTests
    {
        private readonly Mock<IConversionTableDatabase> _mockConversionDatabase;
        private readonly Mock<IAppUsageDatabase> _mockAppUsageDatabase;
        private readonly Mock<IUserActivityLogDatabase> _mockUserActivityLogDatabase;
        private readonly ICO2Service _co2Service;
        private AppUsageModel mockAppData;

        public CalculateCO2DifferenceTests()
        {
            _mockConversionDatabase = new Mock<IConversionTableDatabase>();
            _mockAppUsageDatabase = new Mock<IAppUsageDatabase>();
            _mockUserActivityLogDatabase = new Mock<IUserActivityLogDatabase>();
            _co2Service = new CO2Service(_mockConversionDatabase.Object, _mockAppUsageDatabase.Object, _mockUserActivityLogDatabase.Object);

        }

        [Fact]
        public async Task CalculateCO2DifferenceAsync_WithPositiveDifference_ReturnsCorrectValue()
        {
            // Arrange
            var today = DateTime.Now.Date;
            var yesterday = today - new TimeSpan(1, 0, 0, 0);


            _mockUserActivityLogDatabase.Setup(x => x.GetHighestCO2DailyTotalByDate(It.Is<DateTime>(d => d.Date == today))).ReturnsAsync(new UserActivityLogModel { CO2Total = 50 });
            _mockUserActivityLogDatabase.Setup(x => x.GetHighestCO2DailyTotalByDate(It.Is<DateTime>(d => d.Date == yesterday))).ReturnsAsync(new UserActivityLogModel { CO2Total = 100 });
            _mockUserActivityLogDatabase.Setup(x => x.AddActivityLog(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            var result = await _co2Service.CalculateCO2DifferenceAsync();

            // Assert
            Assert.Equal(50.0, result); // 100 - 50
            _mockUserActivityLogDatabase.Verify(x => x.AddActivityLog(0, 50.0, 0), Times.Once);
        }

        [Fact]
        public async Task CalculateCO2DifferenceAsync_WithNegativeDifference_DoesNotCallAddLog()
        {
            // Arrange
            var today = DateTime.Now.Date;
            var yesterday = today - new TimeSpan(1, 0, 0, 0);


            _mockUserActivityLogDatabase.Setup(x => x.GetHighestCO2DailyTotalByDate(It.Is<DateTime>(d => d.Date == today))).ReturnsAsync(new UserActivityLogModel { CO2Total = 100 });
            _mockUserActivityLogDatabase.Setup(x => x.GetHighestCO2DailyTotalByDate(It.Is<DateTime>(d => d.Date == yesterday))).ReturnsAsync(new UserActivityLogModel { CO2Total = 50 });
            _mockUserActivityLogDatabase.Setup(x => x.AddActivityLog(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            var result = await _co2Service.CalculateCO2DifferenceAsync();

            // Assert
            Assert.Equal(-50.0, result); // 50 - 100
            _mockUserActivityLogDatabase.Verify(x => x.AddActivityLog(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task CalculateCO2DifferenceAsync_WithZeroDifference_DoesNotCallAddLog()
        {
            // Arrange
            var today = DateTime.Now.Date;
            var yesterday = today - new TimeSpan(1, 0, 0, 0);

            _mockUserActivityLogDatabase.Setup(x => x.GetHighestCO2DailyTotalByDate(It.IsAny<DateTime>())).ReturnsAsync(new UserActivityLogModel { CO2Total = 100 });
            _mockUserActivityLogDatabase.Setup(x => x.AddActivityLog(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            var result = await _co2Service.CalculateCO2DifferenceAsync();

            // Assert
            Assert.Equal(0, result);
            _mockUserActivityLogDatabase.Verify(x => x.AddActivityLog(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>()), Times.Never);
        }
    }
}
