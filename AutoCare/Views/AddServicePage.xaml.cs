using AutoCare.ViewModels;

namespace AutoCare.Views;

public partial class AddServicePage : ContentPage
{
    public AddServicePage(AddServiceViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}