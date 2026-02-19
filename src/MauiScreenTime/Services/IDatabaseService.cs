using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Services
{
    public interface IDatabaseService
    {
        Task<string> GetDatabasePathAsync(string dbName);

    }
}
