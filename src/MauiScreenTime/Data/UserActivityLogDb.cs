using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiScreenTime.Services.Interfaces;
using MauiScreenTime.Data.Interfaces;
using System.Diagnostics.CodeAnalysis;

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
        public async Task<double> GetCO2TotalReducedByDate(DateTime inputDate)
        {
            var connection = await GetConnectionAsync();
            var allEntries = await connection.Table<UserActivityLogModel>().ToListAsync();
                
            return allEntries
                .Where(a => a.Date == inputDate.Date)
                .Sum(x => x.CO2TotalReduced);
        }

        public async Task<UserActivityLogModel> GetHighestCO2DailyTotalUsedByDate(DateTime inputDate)
        {
            var connection = await GetConnectionAsync();
            return await connection.Table<UserActivityLogModel>().OrderByDescending(x => x.CO2Total)
                .Where(a => a.Date == inputDate.Date)
                .FirstOrDefaultAsync();
        }


        //public async Task<double> GetCO2TotalByDate(DateTime inputDate) 
        //{
        //    var activity = await GetActivityByDate(inputDate);
        //    return activity?.CO2Total ?? 0;                
        //}
        //public async Task<double> GetCO2TotalReducedByDate(DateTime inputDate)
        //{
        //    var activity = await GetHighestCO2TotalReducedByDate(inputDate);
        //    return activity?.CO2TotalReduced ?? 0;
        //}
        public async Task AddTrees(int treeNumber)
        {
            var connection = await GetConnectionAsync();
            var today = DateTime.UtcNow;

            var activity = await GetActivityByDate(today);

            if (activity != null)
            {
                activity.TreesPlanted += treeNumber;
                activity.Date = DateTime.UtcNow;
                await connection.UpdateAsync(activity);
            }
            else
            {
                await AddActivityLog(0, 0, treeNumber);
            }
        }
        //necessary?
        public async Task AddCO2TotalReducedDaily(double CO2TotalReduced)
        {
            var connection = await GetConnectionAsync();
            var today = DateTime.UtcNow;

            var activity = await GetActivityByDate(today);

            if (activity != null)
            {
                activity.CO2TotalReduced += CO2TotalReduced;
                activity.Date = DateTime.UtcNow;
                await connection.UpdateAsync(activity);
            }
            else
            {
                await AddActivityLog(0, CO2TotalReduced, 0);
            }
        }
        public async Task AddActivityLog(double CO2Total, double CO2TotalReduced, int treesPlanted = 0)
        {
            var connection = await GetConnectionAsync();
            var today = DateTime.UtcNow;

            await connection.InsertAsync(new UserActivityLogModel
            {
                Date = today.Date,
                TimeStamp = today,
                CO2Total = (double)CO2Total,
                CO2TotalReduced = CO2TotalReduced,
                //TreesPlanted += treesPlanted
            }
            );
        }
        public async Task AddActivityLogDEBUG(double CO2Total, double CO2TotalReduced, int treesPlanted)
        {
            var connection = await GetConnectionAsync();
            var today = DateTime.Now.AddDays(-1);
            //var yesterday = today - new TimeSpan(1, 0, 0, 0);

            await connection.InsertAsync(new UserActivityLogModel
            {
                Date = today.Date,
                TimeStamp = today,
                CO2Total = (double)CO2Total,
                CO2TotalReduced = CO2TotalReduced,
                //TreesPlanted += treesPlanted
            }
            );
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
