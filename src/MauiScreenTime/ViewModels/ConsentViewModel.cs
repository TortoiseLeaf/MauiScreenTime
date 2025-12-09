using CommunityToolkit.Mvvm.Input;
using MauiScreenTime.Data;
using MauiScreenTime.Pages;
using MauiScreenTime.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Microsoft.Maui.ApplicationModel.Permissions;


namespace MauiScreenTime.ViewModels
{
    public partial class ConsentViewModel : INotifyPropertyChanged
    {
        private readonly ConsentDatabase _db;
        private readonly IUsageStatsService _usageStatsService;
        private bool _hasConsent;

        public string TermsText { get; set; }

        public ConsentViewModel(ConsentDatabase db, IUsageStatsService usageStatsService)
        {
            _db = db;

            _usageStatsService = usageStatsService;

            // commands for xaml/.cs
            GrantConsentCommand = new Command(async () => await GrantConsent());
            RevokeConsentCommand = new Command(async () => await RevokeConsent());
            DeleteAllCommand = new Command(async () => await DeleteAll());

            _ = LoadTermsAndConditions();
            //Task<bool> hasPermission = CheckAndroidPermissions();
            PageAppearing();
        }

        [RelayCommand]
        private async Task PageAppearing()
        {
            // this needs to refire when the settings page closes. I think doesn't navigate away from the page and that's why.
            await CheckAndroidPermissions();
        }



        public async Task<bool> CheckAndroidPermissions()
        {

            bool hasPermission = await _usageStatsService.HasPermissionAsync();
            

            if (!hasPermission)
            {
                Console.WriteLine("fires has not permission");
                // retry logic
                // remove this alert it is too many
                await Shell.Current.DisplayAlert("Permission Required",
            "The app cannot function without usage stats permission.",
            "OK");
                _usageStatsService.CheckAndRequestPermissionsAsync();

            }
            if (hasPermission)
            {
                System.Diagnostics.Debug.WriteLine("fires has permission");
                //await Shell.Current.DisplayAlert("Permission Granted", "You have granted permissions", "OK");
                // collect usage stats
            }
            return hasPermission;
        }

        public bool HasConsent
        {
            get => _hasConsent;
            set
            {
                _hasConsent = value;
                OnPropertyChanged();
            }
        }


        public ICommand GrantConsentCommand { get; }
        public ICommand RevokeConsentCommand { get; }
        public ICommand DeleteAllCommand { get; }


        private async Task GrantConsent()
        {
            await _db.GrantConsent("1.0");
            HasConsent = true;

            await Shell.Current.GoToAsync(nameof(DashboardPage));

        }

        //Add this into user account settings page?

        private async Task RevokeConsent()
        {
            await _db.RevokeConsent();
            HasConsent = false;
        }


        private async Task DeleteAll()
        {
            await _db.DeleteAllConsents();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task LoadTermsAndConditions()
        {
            try
            {
                var stream = await FileSystem.OpenAppPackageFileAsync("TermsAndConditions.txt");
                var reader = new StreamReader(stream);
                TermsText = await reader.ReadToEndAsync();
                OnPropertyChanged(nameof(TermsText));

            }
            catch (Exception e)
            {
                Console.WriteLine("Could not load Terms and Conditions");
                Console.WriteLine(e.Message);
            }
        }

    }
}