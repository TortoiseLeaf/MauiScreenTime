using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using global::Android.App;
using global::Android.Content;
using MauiScreenTime.Platforms.Android.Workers;
using MauiScreenTime.Platforms.Android.Workers;

namespace MauiScreenTime.Platforms.Android.Receivers
{

    [BroadcastReceiver(Enabled = true, DirectBootAware = false, Exported = false)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (intent?.Action == Intent.ActionBootCompleted)
            {
                // Reschedule after reboot — WorkManager doesn't survive reboots
                // unless you reschedule manually like this
                DailyWorkerScheduler.Schedule();
            }
        }
    }
}