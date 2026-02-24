using MauiScreenTime.Data;
using MauiScreenTime.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MauiScreenTimeTests.DatabaseTests
{
    internal class MockDatabaseService : IDatabaseService
    {        
        public Task<string> GetDatabasePathAsync(string dbName = "testDB")
        {
            //var testPath = Path.Combine(Path.GetTempPath(), dbName);
            return Task.FromResult(":memory:");

        }
    }
    public class UserActivityLogDatabaseTests : IAsyncLifetime
    {
        private UserActivityLogDatabase _database;
        private IDatabaseService _databaseTestService;

        public UserActivityLogDatabaseTests()
        {
            _databaseTestService = new MockDatabaseService();
            _database = new UserActivityLogDatabase(_databaseTestService);
        }

        public async Task InitializeAsync()
        {
            _databaseTestService = new MockDatabaseService();
            _database = new UserActivityLogDatabase(_databaseTestService);
        }

        public async Task DisposeAsync()
        {
            await _database.DisposeAsync();
        }

        [Fact]
        public async Task GetAllActivitiesLogged_ReturnsEmpty_WhenDatabaseIsEmpty()
        {
            var result = await _database.GetAllActivitiesLogged();
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddActivityLog_AddsConsistentRecordToDatabase()
        {
            //Arrange            
            var co2Total = 10.5;
            var co2Saved = 100L;
            var treesPlanted = 3;

            //Act
            await _database.AddActivityLog(co2Total, co2Saved, treesPlanted);
            var result = await _database.GetAllActivitiesLogged();

            //Assert
            Assert.Single(result);
            Assert.Equal(co2Total, result[0].CO2Total);
            Assert.Equal(co2Saved, result[0].CO2SavedDaily);
            Assert.Equal(treesPlanted, result[0].TreesPlanted);
        }

        [Fact]
        public async Task GetActivityById_ReturnsActivity_WithSpecifiedId()
        {
            //arrange
            int id = 1;            
            var co2Total = 10.5;
            var co2Saved = 100L;
            var treesPlanted = 3;
            await _database.AddActivityLog(co2Total, co2Saved, treesPlanted);

            //act
            var result = await _database.GetActivityById(id);

            //assert
            Assert.Equal(id, result.Id);            
            Assert.Equal(co2Total, result.CO2Total);
            Assert.Equal(co2Saved, result.CO2SavedDaily);
            Assert.Equal(treesPlanted, result.TreesPlanted);
        }
        [Fact]
        public async Task GetActivityByDate_ReturnsActivity_WithSpecifiedDate()
        {
            //Arrange
            int id = 1;
            var co2Total = 10.5;
            var co2Saved = 100L;
            var treesPlanted = 3;
            await _database.AddActivityLog(co2Total, co2Saved, treesPlanted);
            var result = await _database.GetActivityById(id);
            var date = result.Date;

            //act
            result = await _database.GetActivityByDate(date);

            //assert
            Assert.Equal(id, result.Id);
            Assert.Equal(date, result.Date);
            Assert.Equal(co2Total, result.CO2Total);
            Assert.Equal(co2Saved, result.CO2SavedDaily);
            Assert.Equal(treesPlanted, result.TreesPlanted);
        }
        [Fact]
        public async Task GetCO2eTotalByDate_ReturnsCO2TotalOfTheDate()
        {
            //arrange
            DateTime todayDate = DateTime.UtcNow.Date;
            int id = 1;
            var co2Total = 10.5;
            var co2Saved = 100L;
            var treesPlanted = 3;
            await _database.AddActivityLog(co2Total, co2Saved, treesPlanted);

            //act
            var result = await _database.GetCO2eTotalByDate(todayDate);

            //assert
            Assert.Equal(co2Total, result);
        }
        [Fact]
        public async Task AddTrees_AddInputTrees_ToTreesPlanted()
        {
            //arrange
            DateTime todayDate = DateTime.UtcNow.Date;
            int id = 1;
            var co2Total = 10.5;
            var co2Saved = 100L;
            var treesPlanted = 3;
            
            await _database.AddActivityLog(co2Total, co2Saved, treesPlanted);

            //act
            await _database.AddTrees(4);
            var result = await _database.GetActivityById(id);

            //assert
            Assert.Equal(7, result.TreesPlanted);
        }

        [Fact]
        public async Task DeleteAllActivities_RemovesAllEntries_FromActivityLogTable()
        {
            //arrange
            DateTime todayDate = DateTime.UtcNow.Date;
            int id = 1;
            var co2Total = 10.5;
            var co2Saved = 100L;
            var treesPlanted = 3;
            await _database.AddActivityLog(co2Total, co2Saved, treesPlanted);

            var result = await _database.GetAllActivitiesLogged();
            Assert.Single(result);
            //act
            await _database.DeleteAllActivitiesLogged();
            result = await _database.GetAllActivitiesLogged();
            //assert
            Assert.Empty(result);
        }
    }
}