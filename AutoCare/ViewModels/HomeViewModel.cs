using AutoCare.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Data;

namespace AutoCare.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty] private string greeting = "";
    [ObservableProperty] private string vehicleName = "No vehicle yet";
    [ObservableProperty] private string plate = "";
    [ObservableProperty] private string odometerText = "-";
    [ObservableProperty] private string healthText = "--";
    [ObservableProperty] private double healthProgress;
    [ObservableProperty] private string dueMessage = "";
    [ObservableProperty] private string alertCount = "0";

    public HomeViewModel(DatabaseService db) => _db = db;

    public async Task LoadAsync()
    {
        var name = Preferences.Get("UserName", "Driver");
        var hour = DateTime.Now.Hour;
        var part = hour < 12 ? "Good morning" : hour < 18 ? "Good afternoon" : "Good evening";
        Greeting = $"{part}, {name}";

        var vehicle = await _db.GetVehicleAsync();
        var records = await _db.GetRecordsAsync();

        if (vehicle == null)
        {
            VehicleName = "No vehicle yet";
            Plate = "Tap here to add your vehicle";
            OdometerText = "-";
            HealthText = "--";
            HealthProgress = 0;
            DueMessage = "Add your vehicle in Profile first";
            AlertCount = "0";
            return;
        }
        var rules = await _db.GetRulesAsync();
        var statuses = MaintenanceService.Calculate(vehicle.CurrentOdometer, records, rules);

        VehicleName = vehicle.MakeModel;
        Plate = vehicle.PlateNumber;
        OdometerText = $"{vehicle.CurrentOdometer:N0} km";

        // health = average "life left" of the services that have a record
        var tracked = statuses.Where(s => s.Level != 3).ToList();
        double health = tracked.Count == 0 ? 1.0 : tracked.Average(s => 1 - s.Progress);
        HealthProgress = health;
        HealthText = $"{health * 100:0}%";

        var urgent = statuses.Where(s => s.Level <= 1)
                             .OrderBy(s => s.Level).ThenByDescending(s => s.Progress)
                             .FirstOrDefault();

        DueMessage = urgent == null ? "All services are on track"
                   : urgent.Level == 0 ? $"{urgent.Name} is overdue!"
                   : $"{urgent.Name} due in {urgent.DueIn}";

        AlertCount = statuses.Count(s => s.Level <= 1).ToString();
    }

    [RelayCommand] private Task GoLogAsync() => Shell.Current.GoToAsync("addservice");
    [RelayCommand] private Task GoAlertsAsync() => Shell.Current.GoToAsync("alerts");
    [RelayCommand] private Task GoCalculatorAsync() => Shell.Current.GoToAsync("//calculator");
    [RelayCommand] private Task GoHistoryAsync() => Shell.Current.GoToAsync("//history");
    [RelayCommand] private Task GoProfileAsync() => Shell.Current.GoToAsync("//profile");
}