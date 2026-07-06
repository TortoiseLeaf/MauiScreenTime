using MauiScreenTime.Data.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Data
{
    public class UserGoalsModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Level { get; set; }
        public double TotalSaved { get; set; }
        public double Progress { get; set; }        
        public DateTime Date { get; set; }
    }

    public class UserGoalsDatabase : IUserGoalsDatabase
    {
        private const string DB_NAME = "user_goals_database.db3";
        private SQLiteAsyncConnection _connection;

        private async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            if (_connection == null)
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, DB_NAME);
                _connection = new SQLiteAsyncConnection(dbPath);
                await _connection.CreateTableAsync<UserGoalsModel>(); //this should equate to 'create table if not exists' so only creates the table if not present
            }
            return _connection;
        }

        public async Task SaveTodayGoals(int level, double totalSaved, double progress)
        {
            var con = await GetConnectionAsync();
            var today = DateTime.Now.Date;

            var existing = await con.Table<UserGoalsModel>()
                .Where(d => d.Date == today)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                await con.InsertAsync(new UserGoalsModel()
                {
                    Level = level,
                    TotalSaved = totalSaved,
                    Progress = progress,                    
                    Date = today
                });
            }
            else
            {
                existing.Level = level;
                existing.TotalSaved = totalSaved;
                existing.Progress = progress;
                await con.UpdateAsync(existing);
            }
        }

        public async Task<UserGoalsModel> GetLastGoals()
        {
            var con = await GetConnectionAsync();
            return await con.Table<UserGoalsModel>()
                .OrderByDescending(d => d.Date)
                .FirstOrDefaultAsync();
        }
    }
}
