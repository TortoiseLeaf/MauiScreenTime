using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiScreenTime.Data;
using MauiScreenTime.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace MauiScreenTime.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IUsageStatsService _usageStatsService;
        public bool hasPermission;

        [ObservableProperty]
        private ObservableCollection<AppUsageModel> appUsageList = new();


        public DashboardViewModel(IUsageStatsService usageStatsService)
        {
            
            _usageStatsService = usageStatsService;

            OnAppearing();

        }

        [RelayCommand]
        private async Task OnAppearing()
        {
            
            hasPermission = await _usageStatsService.HasPermissionAsync();
            if (hasPermission)
            {
                try
                {
                    var usageData = await _usageStatsService.GetAppUsageAsync();

                    appUsageList.Clear();
                    foreach (var app in usageData)
                    {
                        appUsageList.Add(app);
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error collecting data", ex.Message, "OK");
                }
                
            }
        }


        

    }
}