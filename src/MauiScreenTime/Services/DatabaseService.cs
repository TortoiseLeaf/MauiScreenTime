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

        public async Task<string> GetDatabasePathAsync(string dbName)
        {
            if (!string.IsNullOrEmpty(_dbPath))
                return _dbPath;


            var dbFolder = FileSystem.AppDataDirectory;
            _dbPath = Path.Combine(dbFolder, dbName);

            if (!File.Exists(_dbPath))
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(dbName);
                using var fileStream = File.Create(_dbPath);
                await stream.CopyToAsync(fileStream);
            }

            return _dbPath;
        }
    }
}

