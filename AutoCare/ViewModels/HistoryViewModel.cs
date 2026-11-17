using System.Collections.ObjectModel;
using AutoCare.Models;
using AutoCare.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCare.ViewModels;

public partial class HistoryViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    public ObservableCollection<ServiceRecord> Records { get; } = new();

    [ObservableProperty] private string totalText = "RM 0.00";
    [ObservableProperty] private string countText = "0 services logged";

    public HistoryViewModel(DatabaseService db) => _db = db;

    public async Task LoadAsync()
    {
        var list = await _db.GetRecordsAsync();
        Records.Clear();
        foreach (var r in list) Records.Add(r);
        UpdateTotals();
    }

    private void UpdateTotals()
    {
        TotalText = $"RM {Records.Sum(r => r.Cost):N2}";
        CountText = $"{Records.Count} services logged";
    }

    [RelayCommand]
    private async Task DeleteAsync(ServiceRecord record)
    {
        await _db.DeleteRecordAsync(record);
        Records.Remove(record);
        UpdateTotals();
    }

    [RelayCommand]
    private async Task ShareAsync()
    {
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = "AutoCare Summary",
            Text = $"AutoCare summary: {CountText}, total spent {TotalText}."
        });
    }
}