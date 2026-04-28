using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScreenTime.Platforms.Android.Workers
{
    public static class AndroidServiceLocator
    {
        private static IServiceProvider? _serviceProvider;

        public static void Init(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static T GetService<T>() where T : notnull
        {
            // Try MAUI's platform application first
            _serviceProvider ??= IPlatformApplication.Current?.Services;

            if (_serviceProvider == null)
                throw new InvalidOperationException("ServiceLocator not initialized.");

            return _serviceProvider.GetRequiredService<T>();
        }
    }
}