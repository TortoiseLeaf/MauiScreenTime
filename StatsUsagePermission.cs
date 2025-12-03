using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace MauiScreenTime
{
    internal class StatsUsagePermission : BasePlatformPermission
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions => 
            new List<(string permission, bool isRuntime)>
            {
                (Android.Manifest.Permission.PackageUsageStats, false),
            }.ToArray();

#endif
    }
}
