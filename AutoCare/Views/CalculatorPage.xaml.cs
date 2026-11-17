using AutoCare.ViewModels;

namespace AutoCare.Views;

public partial class CalculatorPage : ContentPage
{
    private readonly CalculatorViewModel _vm;

    public CalculatorPage(CalculatorViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }

    private async void OnOdometerUnfocused(object? sender, FocusEventArgs e)
    {
        await _vm.SaveOdometerAsync();
    }
}
