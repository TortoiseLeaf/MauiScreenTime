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
        public int? Id { get; set; } = 0;
        public string? PackageName { get; set; } = string.Empty;
        public string? AppName { get; set; } = string.Empty;
        public DateTime? Date { get; set; } = new DateTime();
        public TimeSpan? UsageTimeMilliseconds { get; set; } = new TimeSpan();
        public long? UsageTimeMinutes { get; set; } = 0;
        public double? CO2e { get; set; } = 0;

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
