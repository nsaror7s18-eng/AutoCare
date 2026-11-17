using AutoCare.ViewModels;

namespace AutoCare.Views;

public partial class ServiceIntervalsPage : ContentPage
{
    private readonly ServiceIntervalsViewModel _vm;

    public ServiceIntervalsPage(ServiceIntervalsViewModel vm)
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