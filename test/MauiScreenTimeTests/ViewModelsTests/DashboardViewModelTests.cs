//using MauiScreenTime.Data;
//using MauiScreenTime.Services;
//using MauiScreenTime.ViewModels;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml.Linq;

//namespace MauiScreenTimeTests.ViewModelsTests
//{
//    public class CO2ServiceTests
//    {
        

//        [Fact]
//        public async Task LoadData_ShouldPopulateDataList_WhenServiceReturnsData()
//        {
//            // Arrange
//            // Arrange
//            var mockUsageStatsService = new Mock<IUsageStatsService>();
//            var mockCO2Service = new Mock<ICO2Service>();

//            var testData = new AppUsageModel { PackageName = "com.android.settings" };
//            mockCO2Service.Setup(x => x.CalculateCO2eAsync(testData)).ReturnsAsync(testData);

//            var viewModel = new DashboardViewModel(mockUsageStatsService.Object, mockCO2Service.Object);

//            // Act
//            await viewModel.GetCO2Coversion();

//            // Assert - Focus on ViewModel behavior, not service calls
//            Assert.Single(viewModel.AppUsageListCO2);
//            Assert.Equal("Test", viewModel.AppUsageList[0].AppName);
//        }

        
//    }
//}
