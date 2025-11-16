using MauiScreenTime.Pages;

namespace MauiScreenTime
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // define routes here
            Routing.RegisterRoute(nameof(ConsentPage), typeof(ConsentPage));
            Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));

        }
    }
}
