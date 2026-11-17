using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCare.Models;

public partial class ServiceChoice : ObservableObject
{
    public string Name { get; set; } = "";

    [ObservableProperty] private bool isSelected;
    [ObservableProperty] private string costText = "";
}
