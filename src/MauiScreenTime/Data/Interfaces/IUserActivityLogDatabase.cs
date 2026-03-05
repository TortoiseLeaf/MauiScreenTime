using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Data.Interfaces
{
    public interface IUserActivityLogDatabase
    {
        Task AddActivityLog(double CO2Total, double CO2TotalReduced, int treesPlanted);

        Task<UserActivityLogModel> GetHighestCO2DailyTotalByDate(DateTime inputDate);

        Task<double> GetCO2TotalReducedByDate(DateTime inputDate);

        Task<UserActivityLogModel> GetActivityByDate(DateTime inputDate);

        Task<List<UserActivityLogModel>> GetAllActivitiesLogged();

        Task<int> GetLatestTreesByDate(DateTime inputDate);

        //Task AddCO2TotalReducedDaily(double CO2TotalReduced);
         Task AddActivityLogDEBUG(double CO2Total, double CO2TotalReduced, int treesPlanted);

    }
}
