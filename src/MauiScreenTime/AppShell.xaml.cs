using MauiScreenTime.Pages;

namespace MauiScreenTime
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ConsentPage), typeof(ConsentPage));
        }
    }
}
