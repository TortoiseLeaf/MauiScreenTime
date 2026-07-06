using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Data.Interfaces
{
    public interface IUserGoalsDatabase
    {
        Task SaveTodayGoals(int level, double totalSaved, double progress);
        Task<UserGoalsModel> GetLastGoals();
    }
}
