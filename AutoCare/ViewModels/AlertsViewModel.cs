using System.Collections.ObjectModel;
using AutoCare.Models;
using AutoCare.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCare.ViewModels;

public partial class AlertsViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private bool _loading;

    public ObservableCollection<AlertItem> Alerts { get; } = new();

    [ObservableProperty] private bool notificationsEnabled = true;
    [ObservableProperty] private string emptyText = "";

    public AlertsViewModel(DatabaseService db) => _db = db;

    public async Task LoadAsync()
    {
        _loading = true;
        NotificationsEnabled = Preferences.Get("AlertsOn", true);
        _loading = false;
        await BuildAlertsAsync();
    }

    // runs automatically whenever the Switch changes
    partial void OnNotificationsEnabledChanged(bool value)
    {
        if (_loading) return;
        Preferences.Set("AlertsOn", value);
        _ = BuildAlertsAsync();
    }

    private async Task BuildAlertsAsync()
    {
        Alerts.Clear();

        if (!NotificationsEnabled)
        {
            EmptyText = "Alerts are turned off.";
            return;
        }

        var vehicle = await _db.GetVehicleAsync();
        var records = await _db.GetRecordsAsync();
        var rules = await _db.GetRulesAsync();
        var statuses = MaintenanceService.Calculate(vehicle?.CurrentOdometer ?? 0, records, rules);

        foreach (var s in statuses.Where(s => s.Level <= 1).OrderBy(s => s.Level).ThenByDescending(s => s.Progress))
        {
            bool overdue = s.Level == 0;
            Alerts.Add(new AlertItem
            {
                Badge = overdue ? "URGENT" : "REMINDER",
                Message = overdue ? $"{s.Name} service is overdue" : $"{s.Name} service window approaching",
                Detail = $"{s.KmText} · {s.DaysText}",
                BadgeColor = Color.FromArgb(overdue ? "#D93025" : "#F29900"),
                CardColor = Color.FromArgb(overdue ? "#FDECEA" : "#FFF6E0")
            });
        }

        int missing = statuses.Count(s => s.Level == 3);
        if (missing > 0)
            Alerts.Add(new AlertItem
            {
                Badge = "INFO",
                Message = $"{missing} services have no record yet",
                Detail = "Log a service so AutoCare can track them",
                BadgeColor = Color.FromArgb("#1A73E8"),
                CardColor = Color.FromArgb("#E8F1FD")
            });

        EmptyText = Alerts.Count == 0 ? "No active alerts. Your vehicle is on track!" : "";
    }
}