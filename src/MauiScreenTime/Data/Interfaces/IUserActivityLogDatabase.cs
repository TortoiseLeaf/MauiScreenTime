using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Data.Interfaces
{
    public interface IUserActivityLogDatabase
    {
        Task AddActivityLog(double CO2Total, double CO2SavedToday, int treesPlanted = 0);

        Task<double> GetCO2eTotalByDate(DateTime inputDate);

        Task<double> GetCO2SavedDaylyByDate(DateTime inputDate);

        Task<UserActivityLogModel> GetActivityByDate(DateTime inputDate);


    }
}
