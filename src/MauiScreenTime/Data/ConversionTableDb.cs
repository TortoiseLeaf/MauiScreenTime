using MauiScreenTime.Data.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiScreenTime.Data
{
    public class ConversionTableModel
    {
        public string PackageName { get; set; }
        public string AppName { get; set; }
        public double CO2Mins { get; set; }
    }

    public class ConversionTableDatabase : IConversionTableDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public ConversionTableDatabase()
        {
            try
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "co2_conversion_table.db");
                _database = new SQLiteAsyncConnection(dbPath);
                _database.CreateTableAsync<ConversionTableModel>().Wait();

                SeedDataIfEmpty();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Error creating Co2Conversion Table: ", ex.Message);
            }

        }


        private async void SeedDataIfEmpty()
        {
            var entries = await _database.Table<ConversionTableModel>().CountAsync();
            if (entries <= 0)
            {

                await _database.InsertAllAsync(new[]
                {
                    new ConversionTableModel { AppName = "Youtube", CO2Mins = 0.46, PackageName = "com.google.android.youtube" },
                    new ConversionTableModel { AppName = "Twitch", CO2Mins = 0.55, PackageName = "tv.twitch.android.app" },
                    new ConversionTableModel { AppName = "Twitter", CO2Mins = 0.6, PackageName = "com.twitter.android" },
                    new ConversionTableModel { AppName = "LinkedIn", CO2Mins = 0.71, PackageName = "com.linkedin.android" },
                    new ConversionTableModel { AppName = "Facebook", CO2Mins = 0.79, PackageName = "com.facebook.katana" },
                    new ConversionTableModel { AppName = "Snapchat", CO2Mins = 0.87, PackageName = "com.snapchat.android" },
                    new ConversionTableModel { AppName = "Instagram", CO2Mins = 1.05, PackageName = "com.instagram.android" },
                    new ConversionTableModel { AppName = "Pinterest", CO2Mins = 1.3, PackageName = "com.pinterest" },
                    new ConversionTableModel { AppName = "Reddit", CO2Mins = 2.48, PackageName = "com.reddit.frontpage" },
                    new ConversionTableModel { AppName = "TikTok", CO2Mins = 2.63, PackageName = "com.zhiliaoapp.musically" },

                });

            }
        }

        public async Task<ConversionTableModel> GetConversionTableEntryByPackageName(string packageName)
        {
            var entry = await _database.Table<ConversionTableModel>()
                        .Where(c => c.PackageName == packageName)
                        .FirstOrDefaultAsync();
            return entry;
        }
    }
}
