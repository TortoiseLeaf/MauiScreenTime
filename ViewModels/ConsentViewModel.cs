using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiScreenTime.Data;

namespace MauiScreenTime.ViewModels
{
    public class ConsentViewModel : INotifyPropertyChanged
    {
        private readonly ConsentDatabase _db;
        private readonly ConversionTableDatabase _dbT;
        private bool _hasConsent;
        private List<ConversionTableModel> mydata;
    

        public ConsentViewModel(ConsentDatabase db, ConversionTableDatabase dbT)
        {
            _db = db;
            _dbT = dbT;

            // commands for xaml/.cs
            GrantConsentCommand = new Command(async () => await GrantConsent());
            RevokeConsentCommand = new Command(async () => await RevokeConsent());
            DeleteAllCommand = new Command(async () => await DeleteAll());
            CheckDbCommand = new Command(async () => await CheckDb());

            _ = LoadConsents();
            
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
        public ICommand CheckDbCommand { get; }

        private async Task LoadConsents()
        {
            HasConsent = await _db.HasConsent();
        }
        private async Task CheckDb()
        {
            mydata = await _dbT.GetData();
        }

        private async Task GrantConsent()
        {
            await _db.GrantConsent("1.0");
            HasConsent = true;

            // redirect to next page
            //Application.Current.MainPage = new AppShell();
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
            await LoadConsents();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}