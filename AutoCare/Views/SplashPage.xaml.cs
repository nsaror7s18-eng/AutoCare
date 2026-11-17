namespace AutoCare.Views;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Bar.ProgressTo(1, 2000, Easing.Linear);   // fills over 2 seconds
        Application.Current!.Windows[0].Page = new AppShell();
    }
}