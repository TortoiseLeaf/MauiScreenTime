using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MauiScreenTime.Data
{
    public class UserConsentDb
    {
        public bool IsGranted { get; set; }
        public DateTime GrantedAt { get; set; }
        public string Version { get; set; } // Track consent version for GDPR compliance
        public DateTime? RevokedAt { get; set; }

        public int DataRetentionDays = 60;
    }

    public class ConsentDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public ConsentDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<UserConsentDb>().Wait();
        }

        // Revisit this because idk
        public async Task<bool> HasConsent()
        {
            var consent = await _database.Table<UserConsentDb>()
                .OrderByDescending(c => c.GrantedAt)
                .FirstOrDefaultAsync();

            return consent?.IsGranted ?? false;
        }

        public async Task GrantConsent(string version = "1.0")
        {
            await _database.InsertAsync(new UserConsentDb
            {
                IsGranted = true,
                GrantedAt = DateTime.UtcNow,
                Version = version
            });
        }

        public async Task RevokeConsent(string version = "1.0")
        {
            var consent = await _database.Table<UserConsentDb>()
                .OrderByDescending(c => c.GrantedAt)
                .FirstOrDefaultAsync();

            if (consent != null)
            {
                await _database.InsertAsync(new UserConsentDb
                {
                    IsGranted = false,
                    RevokedAt = DateTime.UtcNow,
                    Version = version
                });
            }
        }

        public async Task<List<UserConsentDb>> GetConsentHistory()
        {
            return await _database.Table<UserConsentDb>().ToListAsync();
        }

        public async Task DeleteAllConsents()
        {
            await _database.DeleteAllAsync<UserConsentDb>();
        }
    }
}
