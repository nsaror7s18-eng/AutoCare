using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCare.Models;

public partial class RuleItem : ObservableObject
{
    public string ServiceType { get; set; } = "";

    [ObservableProperty] private string kmText = "";
    [ObservableProperty] private string daysText = "";
}