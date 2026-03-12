using MauiScreenTime.Data.Interfaces;
using MauiScreenTime.Services.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Data
{
    public class UserActivityLogModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeStamp { get; set; }
        public double CO2Total { get; set; }
        public double CO2TotalReduced { get; set; }
        public int CO2ReducedProgress { get; set; }
        public int TreesPlanted { get; set; }
    }

    public class UserActivityLogDatabase : IUserActivityLogDatabase
    {
        private const string DB_NAME = "user_activity_log.db3";
        private readonly IDatabaseService _databaseService;
        private SQLiteAsyncConnection _connection;

        public UserActivityLogDatabase(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        private async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            if (_connection == null)

            {
                string dbPath = await _databaseService.GetDatabasePathAsync(DB_NAME);

                _connection = new SQLiteAsyncConnection(dbPath);
                await _connection.CreateTableAsync<UserActivityLogModel>();
            }
            return _connection;
        }
        public async Task<List<UserActivityLogModel>> GetAllActivitiesLogged() 
        {
            var connection = await GetConnectionAsync();
            return await connection.Table<UserActivityLogModel>()
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }
        public async Task<UserActivityLogModel> GetActivityById(int id)
        {
            var connection = await GetConnectionAsync();
            return await connection.Table<UserActivityLogModel>()
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task<UserActivityLogModel> GetActivityByDate(DateTime inputDate) 
        {
            var connection = await GetConnectionAsync();                       
            return await connection.Table<UserActivityLogModel>().OrderByDescending(x => x.TimeStamp)
                .Where(a => a.Date == inputDate.Date)
                .FirstOrDefaultAsync();
        }
        public async Task<double> GetCO2TotalReduced()
        {
            var connection = await GetConnectionAsync();
            var allEntries = await connection.Table<UserActivityLogModel>().ToListAsync();
                
            return allEntries
        
                .Sum(x => x.CO2TotalReduced);
        }

        public async Task<UserActivityLogModel> GetHighestCO2DailyTotalByDate(DateTime inputDate)
        {
            var connection = await GetConnectionAsync();
            return await connection.Table<UserActivityLogModel>().OrderByDescending(x => x.CO2Total)
                .Where(a => a.Date == inputDate.Date)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetLatestTreesByDate(DateTime inputDate)
        {
            var connection = await GetConnectionAsync();
            var allEntries = await connection.Table<UserActivityLogModel>().ToListAsync();
            return allEntries
                .Where(a => a.Date == inputDate.Date)
                .Sum(x => x.TreesPlanted);
        }
        public async Task<int> GetTotalCO2ReducedProgress()
        {
            var connection = await GetConnectionAsync();
            var allEntries = await connection.Table<UserActivityLogModel>().ToListAsync();
            return allEntries
                //.Where(a => a.Date == inputDate.Date)
                .Sum(x => x.CO2ReducedProgress);
        }

        public async Task AddActivityLog(double CO2Total, double CO2TotalReduced, int CO2ReducedProgress, int treesPlanted)
        {
            var connection = await GetConnectionAsync();
            var today = DateTime.UtcNow;

            await connection.InsertAsync(new UserActivityLogModel
            {
                Date = today.Date,
                TimeStamp = today,
                CO2Total = (double)CO2Total,
                CO2TotalReduced = CO2TotalReduced,
                CO2ReducedProgress = CO2ReducedProgress,
                TreesPlanted = treesPlanted
            }
            );

            var totalReducedProgress = await GetTotalCO2ReducedProgress();

            if (totalReducedProgress >= 200)
            {
                var all = await connection.Table<UserActivityLogModel>().ToListAsync();

                // clear all ReducedProgress data
                foreach (var entry in all)
                {
                    entry.CO2ReducedProgress = 0;
                    await connection.UpdateAsync(entry);
                }

                // add 200 to CO2TotalReduced and increment trees
                await connection.InsertAsync(new UserActivityLogModel
                {
                    Date = today.Date,
                    TimeStamp = today,
                    CO2Total = 0,
                    CO2TotalReduced = CO2TotalReduced + 200,
                    CO2ReducedProgress = 0,
                    TreesPlanted = treesPlanted + 1
                }
            );
            }

        }

        public async Task DeleteAllActivitiesLogged() 
        {
            var connection = await GetConnectionAsync();
            await connection.DeleteAllAsync<UserActivityLogModel>();
        }

        public async Task DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
                _connection = null;
            }
        }
    }
}


//possibly needed methods
/*
         public async Task<List<UserActivityLogModel>> GetActivitiesByDateRange(DateTime startDate, DateTime endDate) 
        { 
            var connection = await GetConnectionAsync();
            return await connection.Table<UserActivityLogModel>()
                .Where(a => a.Date >= startDate && a.Date <= endDate)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }
 */
