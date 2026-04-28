using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using AndroidX.Work;
using global::Android.Content;
using MauiScreenTime.Services.Interfaces;

namespace MauiScreenTime.Platforms.Android.Workers
{
    public class MauiWorkerFactory : WorkerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public MauiWorkerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override ListenableWorker? CreateWorker(
            Context appContext,
            string workerClassName,
            WorkerParameters workerParameters)
        {
            // Match the requested worker class and resolve from DI
            if (workerClassName == Java.Lang.Class.FromType(typeof(DailyWorker)).CanonicalName)
            {
                var myService = _serviceProvider.GetRequiredService<IDailyWorkerService>();
                return new DailyWorker(appContext, workerParameters, myService);
            }

            return null; // Fall back to default factory for other workers
        }
    }
}