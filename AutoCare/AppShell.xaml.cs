namespace AutoCare;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("addservice", typeof(Views.AddServicePage));
        Routing.RegisterRoute("alerts", typeof(Views.AlertsPage));
        Routing.RegisterRoute("about", typeof(Views.AboutPage));
        Routing.RegisterRoute("intervals", typeof(Views.ServiceIntervalsPage));

        Navigating += OnNavigating;
    }

    // The middle tab opens the form on top instead of acting like a normal tab
    private void OnNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        var target = e.Target?.Location.OriginalString ?? "";
        if (target.EndsWith("/newservice"))
        {
            e.Cancel();
            MainThread.BeginInvokeOnMainThread(async () => await GoToAsync("addservice"));
        }
    }
}
