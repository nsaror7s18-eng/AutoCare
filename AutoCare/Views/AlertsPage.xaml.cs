using AutoCare.ViewModels;

namespace AutoCare.Views;

public partial class AlertsPage : ContentPage
{
    private readonly AlertsViewModel _vm;

    public AlertsPage(AlertsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}