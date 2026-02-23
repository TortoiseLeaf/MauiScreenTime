using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiScreenTime.Data.Interfaces;
using SQLite;

namespace MauiScreenTime.Data
{
    public class AppUsageModel
    {
        public int Id {  get; set; }
        public string PackageName { get; set; }
        public string AppName { get; set; }
        public DateTime Date {  get; set; }
        public TimeSpan UsageTimeMilliseconds { get; set; }
        public long UsageTimeMinutes { get; set; }
        public double CO2e { get; set; }

    }

    //appusagedb controller placeholder
    public class AppUsageDatabase : IAppUsageDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public AppUsageDatabase()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app_usage_table.db");

            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<AppUsageModel>().Wait();
        }
    }
}
