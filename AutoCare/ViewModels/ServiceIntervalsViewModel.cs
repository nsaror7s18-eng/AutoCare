using System.Collections.ObjectModel;
using AutoCare.Models;
using AutoCare.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCare.ViewModels;

public partial class ServiceIntervalsViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    public ObservableCollection<RuleItem> Items { get; } = new();

    public ServiceIntervalsViewModel(DatabaseService db) => _db = db;

    public async Task LoadAsync()
    {
        var rules = await _db.GetRulesAsync();
        Items.Clear();
        foreach (var r in rules)
            Items.Add(new RuleItem
            {
                ServiceType = r.ServiceType,
                KmText   = r.IntervalKm   > 0 ? r.IntervalKm.ToString()   : "",
                DaysText = r.IntervalDays > 0 ? r.IntervalDays.ToString() : ""
            });
    }

    // empty box = 0 (not used); otherwise must be a whole number >= 0
    private static bool TryRead(string? text, out int value)
    {
        if (string.IsNullOrWhiteSpace(text)) { value = 0; return true; }
        return int.TryParse(text, out value) && value >= 0;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        // 1) validate every row first, so we never save half the data
        foreach (var item in Items)
        {
            if (!TryRead(item.KmText, out int km) || !TryRead(item.DaysText, out int days) ||
                (km == 0 && days == 0))
            {
                await Shell.Current.DisplayAlertAsync("Invalid input",
                    $"Check {item.ServiceType}: use whole numbers and fill in at least km or days.", "OK");
                return;
            }
        }

        // 2) save
        foreach (var item in Items)
        {
            TryRead(item.KmText, out int km);
            TryRead(item.DaysText, out int days);
            await _db.SaveRuleAsync(new ServiceRule
            {
                ServiceType = item.ServiceType,
                IntervalKm = km,
                IntervalDays = days
            });
        }

        await Shell.Current.DisplayAlertAsync("Saved", "Service intervals updated.", "OK");
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task ResetAsync()
    {
        bool yes = await Shell.Current.DisplayAlertAsync("Reset",
            "Restore the default intervals?", "Yes", "Cancel");
        if (!yes) return;

        await _db.ResetRulesAsync();
        await LoadAsync();
    }
}
