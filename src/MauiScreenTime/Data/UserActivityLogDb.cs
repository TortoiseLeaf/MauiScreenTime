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
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace MauiScreenTime.Data
{
    public class UserActivityLogModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime? TimeStamp { get; set; }
        public double CO2Total { get; set; }
        public double CO2TotalReduced { get; set; }
        public int ProgressBar { get; set; }
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
                //.Where(a => a.Date == inputDate.Date)
                .Sum(x => x.TreesPlanted);
        }
        /*public async Task<int> DisplayLatestProgressBar()
        {
            var connection = await GetConnectionAsync();
            var allEntries = await connection.Table<UserActivityLogModel>().ToListAsync();
            var todaysEntries = allEntries
                //.Where(a => a.Date.Date == DateTime.UtcNow.Date)
                .OrderByDescending(x => x.ProgressBar)
                .ToList();

            foreach (var entry in todaysEntries)
            {
                if (entry.ProgressBar < 200)
                    return entry.ProgressBar;
            }

            return 0;
        } */

        public async Task<int> DisplayLatestProgressBar()
        {
            return await GetLatestProgressBar();
        }
        public async Task<int> GetLatestProgressBar()
        {
            var connection = await GetConnectionAsync();
            var allEntries = await connection.Table<UserActivityLogModel>().ToListAsync();
            return allEntries
                //.Where(a => a.Date.Date == DateTime.UtcNow.Date)
                .OrderByDescending(x => x.TimeStamp)
                .FirstOrDefault()?.ProgressBar ?? 0;
            
        }
       
        public async Task<List<UserActivityLogModel>> GetAllCO2ReducedProgressEntries()
        {
            var connection = await GetConnectionAsync();
            var allEntries = await connection.Table<UserActivityLogModel>().ToListAsync();
            return allEntries;
                
        }

        public async Task DEBUG(double CO2Total, double CO2TotalReduced, int ProgressBar, int treesPlanted)
        {
            var connection = await GetConnectionAsync();
            var today = DateTime.Now.AddDays(-1);

            await connection.InsertAsync(new UserActivityLogModel
            {
                Date = today.Date,
                TimeStamp = today,
                CO2Total = (double)CO2Total,
                CO2TotalReduced = CO2TotalReduced,
                ProgressBar = ProgressBar,
                TreesPlanted = treesPlanted
            }
            );
        }
        public async Task DEBUG2(double CO2Total, double CO2TotalReduced, int ProgressBar, int treesPlanted)
        {
            var connection = await GetConnectionAsync();
            var today = DateTime.Now.AddDays(-2);

            await connection.InsertAsync(new UserActivityLogModel
            {
                Date = today.Date,
                TimeStamp = today,
                CO2Total = (double)CO2Total,
                CO2TotalReduced = CO2TotalReduced,
                ProgressBar = ProgressBar,
                TreesPlanted = treesPlanted
            }
            );
        }
        public async Task AddActivityLog(double CO2Total, double CO2TotalReduced, int ProgressBar, int treesPlanted)
        {
            var connection = await GetConnectionAsync();
            var today = DateTime.UtcNow;

            await connection.InsertAsync(new UserActivityLogModel
            {
                Date = today.Date,
                TimeStamp = today,
                CO2Total = (double)CO2Total,
                CO2TotalReduced = CO2TotalReduced,
                ProgressBar = ProgressBar,
                TreesPlanted = treesPlanted
            }
            );

        }
        public async Task AddActivityLogDEBUG(double CO2Total, double CO2TotalReduced, int ProgressBar, int treesPlanted)
        {
            var connection = await GetConnectionAsync();
            var yesterday = DateTime.UtcNow.AddDays(-1);

            await connection.InsertAsync(new UserActivityLogModel
            {
                Date = yesterday.Date,
                TimeStamp = yesterday,
                CO2Total = (double)CO2Total,
                CO2TotalReduced = CO2TotalReduced,
                ProgressBar = ProgressBar,
                TreesPlanted = treesPlanted
            }
            );

        }

        public async Task UpdateProgressBar() {

            Console.WriteLine("here updating progress bar from user db");

            var connection = await GetConnectionAsync();
            var today = DateTime.UtcNow;

            var progressBarValue = await GetLatestProgressBar();


            if (progressBarValue >= 200)
            {
                // add bar to total with 200 to CO2TotalReduced and increment tree
                await connection.InsertAsync(new UserActivityLogModel
                {
                    Date = today.Date,
                    TimeStamp = today,
                    CO2Total = 0,
                    CO2TotalReduced = 200,
                    ProgressBar = progressBarValue - 200,
                    TreesPlanted = 1
                });
                
                // set the remainder
                //var remainder = progressBarValue - 200;
                
                //// clear the progress bar - FIX NO NEED TO CLEAR ROWS OUT ANYMORE
                //var all = await connection.Table<UserActivityLogModel>().ToListAsync();

                //foreach (var entry in all)
                //{
                //    entry.ProgressBar = 0;
                //    await connection.UpdateAsync(entry);
                //}

                // Save remainder back to progress bar
                /*if (remainder > 0)
                {
                    await connection.InsertAsync(new UserActivityLogModel
                    {
                        Date = today.Date,
                        ProgressBar =+ remainder,
                    });
                    Console.WriteLine("here remainder added to progressbar: " + remainder);

                }*/

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
