//using Android.AdServices.Common;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiScreenTime.Data
{
    [Table("user_consent")]

    public class UserConsentModel
    {
        [Column("is_granted")]
        public bool IsGranted { get; set; }

        [Column("granted_at")]
        public DateTime GrantedAt { get; set; }

        [Column("version")]
        public string Version { get; set; } // Track consent version for GDPR compliance

        [Column("revoked_at")]
        public DateTime? RevokedAt { get; set; }

        public int DataRetentionDays = 60;

    }

    public class ConsentDatabase
    {

        private const string DB_NAME = "user_consent.db3";
        private readonly SQLiteAsyncConnection _connection;


        public ConsentDatabase()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
            _connection.CreateTableAsync<UserConsentModel>();
        }

        // Revisit this because idk
        public async Task<bool> HasConsent()
        {
            var consent = await _connection.Table<UserConsentModel>()
                .OrderByDescending(c => c.GrantedAt)
                .FirstOrDefaultAsync();

            return consent?.IsGranted ?? false;
        }

        public async Task GrantConsent(string version = "1.0")
        {
            await _connection.InsertAsync(new UserConsentModel
            {
                IsGranted = true,
                GrantedAt = DateTime.UtcNow,
                Version = version
            });
        }

        public async Task RevokeConsent(string version = "1.0")
        {
            var consent = await _connection.Table<UserConsentModel>()
                .OrderByDescending(c => c.GrantedAt)
                .FirstOrDefaultAsync();

            if (consent != null)
            {
                await _connection.InsertAsync(new UserConsentModel
                {
                    IsGranted = false,
                    RevokedAt = DateTime.UtcNow,
                    Version = version
                });
            }
        }

        public async Task<List<UserConsentModel>> GetConsentHistory()
        {
            return await _connection.Table<UserConsentModel>().ToListAsync();
        }

        public async Task DeleteAllConsents()
        {
            await _connection.DeleteAllAsync<UserConsentModel>();
        }
    }
}
