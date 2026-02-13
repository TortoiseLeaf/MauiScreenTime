using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Data.Interfaces
{
    public interface IConsentDatabase
    {
        Task<bool> HasConsent();
        Task GrantConsent(string version = "1.0");
        Task RevokeConsent(string version = "1.0");
        Task<List<UserConsentModel>> GetConsentHistory();
        Task DeleteAllConsents();
    }
}
