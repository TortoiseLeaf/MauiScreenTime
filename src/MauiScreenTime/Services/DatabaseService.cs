using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Services
{
    public class DatabaseService
    {
    private string _dbPath;

        public async Task<string> GetDatabasePathAsync()
        {
            if (!string.IsNullOrEmpty(_dbPath))
                return _dbPath;

// make the method reusable by adding these as params
            var dbFolder = FileSystem.AppDataDirectory;
            var dbName = "user_consent.db3";
            _dbPath = Path.Combine(dbFolder, dbName);

            // check if db file is in the app data already
            if (!File.Exists(_dbPath))
            {
                // read from resources and write to app data directory
                using var stream = await FileSystem.OpenAppPackageFileAsync(dbName);
                using var fileStream = File.Create(_dbPath);
                await stream.CopyToAsync(fileStream);
            }

            return _dbPath;
        }
    }
}

