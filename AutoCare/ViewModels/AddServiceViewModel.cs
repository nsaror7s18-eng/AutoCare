using System.Collections.ObjectModel;
using AutoCare.Models;
using AutoCare.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCare.ViewModels;

public partial class AddServiceViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    public ObservableCollection<ServiceChoice> Choices { get; } = new();

    [ObservableProperty] private DateTime serviceDate = DateTime.Today;
    [ObservableProperty] private string odometer = "";
    [ObservableProperty] private string notes = "";

    public AddServiceViewModel(DatabaseService db)
    {
        _db = db;
        foreach (var rule in DatabaseService.DefaultRules)
            Choices.Add(new ServiceChoice { Name = rule.ServiceType });
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var selected = Choices.Where(c => c.IsSelected).ToList();
        if (selected.Count == 0)
        {
            await Shell.Current.DisplayAlertAsync("No service selected",
                "Tick at least one service.", "OK");
            return;
        }

        if (!int.TryParse(Odometer, out int odo) || odo < 0)
        {
            await Shell.Current.DisplayAlertAsync("Invalid input",
                "Enter a valid odometer reading.", "OK");
            return;
        }

        // build every record first, so nothing is saved if one cost is wrong
        var records = new List<ServiceRecord>();
        foreach (var c in selected)
        {
            double cost = 0;
            if (!string.IsNullOrWhiteSpace(c.CostText) &&
                (!double.TryParse(c.CostText, out cost) || cost < 0))
            {
                await Shell.Current.DisplayAlertAsync("Invalid input",
                    $"Enter a valid cost for {c.Name}.", "OK");
                return;
            }

            records.Add(new ServiceRecord
            {
                ServiceType = c.Name,
                ServiceDate = ServiceDate,
                Cost = cost,
                Odometer = odo,
                Notes = Notes ?? ""
            });
        }

        foreach (var r in records)
            await _db.AddRecordAsync(r);

        // keep the vehicle's odometer up to date
        var vehicle = await _db.GetVehicleAsync();
        if (vehicle != null && odo > vehicle.CurrentOdometer)
        {
            vehicle.CurrentOdometer = odo;
            await _db.SaveVehicleAsync(vehicle);
        }

        await Shell.Current.DisplayAlertAsync("Saved", $"{records.Count} service record(s) saved.", "OK");
        await Shell.Current.GoToAsync("//history");
    }
}
