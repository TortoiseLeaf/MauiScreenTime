using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Data.Interfaces
{
    public interface IDailyCO2Database
    {
        Task SaveTodayTotalAsync(double co2Total);

        Task<List<DailyCO2Model>> GetAllOrderedByDateAsync();
    }
}
