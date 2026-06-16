using MauiScreenTime.Data.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Data
{
    public class DailyCO2Model
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public double CO2Total { get; set; }

    }

    public class DailyCO2DataBase : IDailyCO2Database
    {
        private const string DB_NAME = "daily_co2_database.db3";
        private SQLiteAsyncConnection _connection;
        //Not using DataBaseService because we are not loading a DB from Resources
        
        private async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            if (_connection == null)
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, DB_NAME);
                _connection = new SQLiteAsyncConnection(dbPath);
                await _connection.CreateTableAsync<DailyCO2Model>(); //this should equate to 'create table if not exists' so only creates the table if not present
            }
            return _connection;
        }

        //Insert daily total, should overwrite if a row for current day already exists
        public async Task SaveTodayTotalAsync(double co2Total)
        {
            var con = await GetConnectionAsync();
            var today = DateTime.Now.Date;

            var existing = await con.Table<DailyCO2Model>()
                .Where(d => d.Date == today)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                await con.InsertAsync(new DailyCO2Model
                {
                    Date = today,
                    CO2Total = co2Total
                });
            }
            else
            {
                existing.CO2Total = co2Total;
                await con.UpdateAsync(existing);
            }
        }

        public async Task<List<DailyCO2Model>> GetAllOrderedByDateAsync()
        {
            var con = await GetConnectionAsync();

            return await con.Table<DailyCO2Model>()
                .OrderBy(d => d.Date)
                .ToListAsync();
        }
    }
}
