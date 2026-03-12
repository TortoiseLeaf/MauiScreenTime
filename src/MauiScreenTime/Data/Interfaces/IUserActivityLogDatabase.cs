using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Data.Interfaces
{
    public interface IUserActivityLogDatabase
    {
        Task AddActivityLog(double CO2Total, double CO2TotalReduced, int CO2ReducedProgress, int treesPlanted);

        Task<UserActivityLogModel> GetHighestCO2DailyTotalByDate(DateTime inputDate);

        Task<double> GetCO2TotalReduced();

        Task<UserActivityLogModel> GetActivityByDate(DateTime inputDate);

        Task<List<UserActivityLogModel>> GetAllActivitiesLogged();

        Task<int> GetLatestTreesByDate(DateTime inputDate);

        Task<int> GetTotalCO2ReducedProgress();

    }
}
