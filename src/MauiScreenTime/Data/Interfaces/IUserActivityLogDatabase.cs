using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Data.Interfaces
{
    public interface IUserActivityLogDatabase
    {
        Task AddActivityLog(double CO2Total, long CO2SavedToday, int treesPlanted = 0);
    }
}
