using AutoCare.Models;
using AutoCare.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCare.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private const string OtherOption = "Other (type manually)";

    private readonly DatabaseService _db;
    private Vehicle _vehicle = new();

    public List<string> CarModels { get; } = new()
    {
        "Perodua Myvi", "Perodua Axia", "Perodua Bezza",
        "Proton Saga", "Proton X50",
        "Honda City", "Toyota Vios",
        OtherOption
    };

    [ObservableProperty] private string ownerName = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsOtherSelected))]
    private string? selectedModel;
    [ObservableProperty] private string customModel = "";
    [ObservableProperty] private string plateNumber = "";
    [ObservableProperty] private string year = "";
    [ObservableProperty] private string odometer = "";

    public bool IsOtherSelected => SelectedModel == OtherOption;

    public ProfileViewModel(DatabaseService db) => _db = db;

    public async Task LoadAsync()
    {
        OwnerName = Preferences.Get("UserName", "");
        var v = await _db.GetVehicleAsync();
        if (v == null) return;

        _vehicle = v;

        if (CarModels.Contains(v.MakeModel) && v.MakeModel != OtherOption)
            SelectedModel = v.MakeModel;
        else
        {
            SelectedModel = OtherOption;
            CustomModel = v.MakeModel;
        }

        PlateNumber = v.PlateNumber;
        Year = v.Year.ToString();
        Odometer = v.CurrentOdometer.ToString();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        string model = IsOtherSelected ? CustomModel.Trim() : (SelectedModel ?? "");

        if (string.IsNullOrWhiteSpace(model) || string.IsNullOrWhiteSpace(PlateNumber)
            || !int.TryParse(Year, out int y) || y < 1980 || y > DateTime.Now.Year + 1
            || !int.TryParse(Odometer, out int odo) || odo < 0)
        {
            await Shell.Current.DisplayAlertAsync("Invalid input", "Please fill in all fields correctly.", "OK");
            return;
        }

        _vehicle.MakeModel = model;
        _vehicle.PlateNumber = PlateNumber.Trim().ToUpper();
        _vehicle.Year = y;
        _vehicle.CurrentOdometer = odo;

        await _db.SaveVehicleAsync(_vehicle);
        Preferences.Set("UserName", string.IsNullOrWhiteSpace(OwnerName) ? "Driver" : OwnerName.Trim());

        await Shell.Current.DisplayAlertAsync("Saved", "Vehicle details updated.", "OK");
    }

    [RelayCommand]
    private Task OpenIntervalsAsync() => Shell.Current.GoToAsync("intervals");

    [RelayCommand]
    private Task OpenAboutAsync() => Shell.Current.GoToAsync("about");
}
