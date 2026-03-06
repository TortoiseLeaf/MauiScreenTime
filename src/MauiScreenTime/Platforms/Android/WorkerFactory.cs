//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Android.Content;
//using AndroidX.Work;
//using global::Android.Content;
//using MauiScreenTime.Services.Interfaces;

//namespace MauiScreenTime.Platforms.Android
//{
    
//    public class MauiWorkerFactory : WorkerFactory
//    {
//        private readonly IDailyWorkerService _dailyWorkerService;

//        public MauiWorkerFactory(IDailyWorkerService dailyWorkerService)
//        {
//            _dailyWorkerService = dailyWorkerService;

//        }

//        public override ListenableWorker? CreateWorker(
//            Context appContext,
//            string workerClassName,
//            WorkerParameters workerParameters)
//        {
//            if (workerClassName == Java.Lang.Class.FromType(typeof(DailyWorker)).CanonicalName)
//                return new DailyWorker(appContext, workerParameters, _dailyWorkerService);

//            return null; // Fall back to default factory for other workers
//        }
//    }
//}
