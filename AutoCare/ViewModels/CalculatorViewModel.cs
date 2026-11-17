using System.Collections.ObjectModel;
using AutoCare.Models;
using AutoCare.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCare.ViewModels;

public partial class CalculatorViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private Vehicle? _vehicle;
    private bool _ready;
    private int _version;

    public ObservableCollection<MaintenanceStatus> Items { get; } = new();

    [ObservableProperty] private string odometer = "";
    [ObservableProperty] private string noRecordText = "";

    public CalculatorViewModel(DatabaseService db) => _db = db;

    public async Task LoadAsync()
    {
        _ready = false;
        _vehicle = await _db.GetVehicleAsync();
        Odometer = _vehicle?.CurrentOdometer.ToString() ?? "";
        _ready = true;
        await RefreshAsync();
    }

    // runs automatically every time the odometer text changes
    partial void OnOdometerChanged(string value)
    {
        if (_ready) _ = RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        int myVersion = ++_version;

        int.TryParse(Odometer, out int odo);
        var records = await _db.GetRecordsAsync();
        var rules = await _db.GetRulesAsync();
        var all = MaintenanceService.Calculate(odo, records, rules);

        if (myVersion != _version) return;   // a newer calculation started, so drop this one

        Items.Clear();
        foreach (var s in all.Where(s => s.Level != 3).OrderBy(s => s.Level).ThenByDescending(s => s.Progress))
            Items.Add(s);

        int missing = all.Count(s => s.Level == 3);
        NoRecordText = missing > 0
            ? $"{missing} other services have no record yet. Log a service to start tracking them."
            : "";
    }

    // called when the user leaves the odometer box
    public async Task SaveOdometerAsync()
    {
        if (_vehicle == null) return;
        if (!int.TryParse(Odometer, out int odo) || odo < 0) return;

        _vehicle.CurrentOdometer = odo;
        await _db.SaveVehicleAsync(_vehicle);
    }
}
