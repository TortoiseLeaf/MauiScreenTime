using MauiScreenTime.Data;
using MauiScreenTime.Pages;
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


namespace MauiScreenTime.ViewModels
{
    public class ConsentViewModel : INotifyPropertyChanged
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
            CheckAndroidPermissions();
        }

        public async void CheckAndroidPermissions()
        {

            bool hasPermission = await _usageStatsService.CheckAndRequestPermissionsAsync();
            Console.WriteLine("CHECKANDROID FIRING: ", hasPermission);
            if (hasPermission)
            {
                // collect usage stats
            }
            
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