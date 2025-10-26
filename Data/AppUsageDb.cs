using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Data
{
    public class AppUsageDb
    {
        public int Id {  get; set; }
        public string PackageName { get; set; }
        public DateTime Date {  get; set; }
        public long UsageTimeMilliseconds { get; set; }
    }
}
