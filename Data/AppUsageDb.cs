using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MauiScreenTime.Data
{
    public class AppUsageDb
    {
        public int Id {  get; set; }
        public string PackageName { get; set; }
        public DateTime Date {  get; set; }
        public long UsageTimeMilliseconds { get; set; }
    }

    //appusagedb controller placeholder
    public class AppUsageDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public AppUsageDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<AppUsageDb>().Wait();
        }
    }
}
