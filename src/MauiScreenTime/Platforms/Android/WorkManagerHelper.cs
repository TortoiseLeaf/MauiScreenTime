//using Android.Content;
//using AndroidX.Work;
//using global::Android.Content;
//using MauiScreenTime.Helpers;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MauiScreenTime.Platforms.Android
//{
    
//    public static class WorkManagerHelper
//    {
//        public static void ScheduleCalculateTotalCO2(Context context)
//        {
//            //var now = DateTime.Now;
//            //var nextRun = DateTime.Today.AddHours(23);

//            //if (now > nextRun)
//            //    nextRun = nextRun.AddDays(1);

//            //var delay = nextRun - now;

//            var delay = ScheduleCalculator.CalculateDelayUntilNextRun(DateTime.Now, 23);

//            var constraints = new Constraints.Builder()
//                .SetRequiredNetworkType(NetworkType.NotRequired)
//                .Build();

//            //var workRequest = (PeriodicWorkRequest)new PeriodicWorkRequest.Builder(
//            //        Java.Lang.Class.FromType(typeof(DailyWorker)),
//            //        15,
//            //        Java.Util.Concurrent.TimeUnit.Minutes!)
//            //    //.SetInitialDelay((long)delay.TotalMinutes, Java.Util.Concurrent.TimeUnit.Minutes!)
//            //    .SetConstraints(constraints)
//            //    .Build();

//            //WorkManager
//            //    .GetInstance(context)
//            //    .EnqueueUniquePeriodicWork(
//            //        "total_CO2_23h",
//            //        ExistingPeriodicWorkPolicy.CancelAndReenqueue,
//            //        workRequest);

//            var workRequest = (PeriodicWorkRequest)new PeriodicWorkRequest.Builder(
//                    Java.Lang.Class.FromType(typeof(DailyWorker)),
//                    24,
//                    Java.Util.Concurrent.TimeUnit.Hours!)
//                .SetInitialDelay((long)delay.TotalMinutes, Java.Util.Concurrent.TimeUnit.Minutes!)
//                .SetConstraints(constraints)
//                .Build();

//            WorkManager
//                .GetInstance(context)
//                .EnqueueUniquePeriodicWork(
//                    "total_CO2_23h",
//                    ExistingPeriodicWorkPolicy.Keep,
//                    workRequest);

//        }
//    }
//}
