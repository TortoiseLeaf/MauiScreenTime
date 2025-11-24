using MauiScreenTime.Pages;

namespace MauiScreenTime
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }
        private async void OnConsentClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(ConsentPage));
        }
        private void OnCounterClicked(object? sender, EventArgs e)
        {

        }
    }
}
