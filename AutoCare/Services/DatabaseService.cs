using SQLite;
using AutoCare.Models;

namespace AutoCare.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _db;

    // Default intervals (0 = this limit is not used)
    public static readonly List<ServiceRule> DefaultRules = new()
    {
        new() { ServiceType = "Engine Oil",         IntervalKm = 10000, IntervalDays = 180  },
        new() { ServiceType = "Brake Pads",         IntervalKm = 30000, IntervalDays = 0    },
        new() { ServiceType = "Tyre Rotation",      IntervalKm = 10000, IntervalDays = 180  },
        new() { ServiceType = "Battery",            IntervalKm = 0,     IntervalDays = 1095 },
        new() { ServiceType = "Air Filter",         IntervalKm = 20000, IntervalDays = 365  },
        new() { ServiceType = "General Inspection", IntervalKm = 15000, IntervalDays = 365  },
    };

    private async Task InitAsync()
    {
        if (_db != null) return;

        var path = Path.Combine(FileSystem.AppDataDirectory, "autocare.db3");
        _db = new SQLiteAsyncConnection(path);
        await _db.CreateTableAsync<Vehicle>();
        await _db.CreateTableAsync<ServiceRecord>();
        await _db.CreateTableAsync<ServiceRule>();
    }

    // ---------- Vehicle ----------
    public async Task<Vehicle?> GetVehicleAsync()
    {
        await InitAsync();
        return await _db!.Table<Vehicle>().FirstOrDefaultAsync();
    }

    public async Task SaveVehicleAsync(Vehicle v)
    {
        await InitAsync();
        if (v.Id == 0) await _db!.InsertAsync(v);
        else await _db!.UpdateAsync(v);
    }

    // ---------- Service records ----------
    public async Task<List<ServiceRecord>> GetRecordsAsync()
    {
        await InitAsync();
        return await _db!.Table<ServiceRecord>()
                         .OrderByDescending(r => r.ServiceDate)
                         .ThenByDescending(r => r.Id)
                         .ToListAsync();
    }

    public async Task AddRecordAsync(ServiceRecord r)
    {
        await InitAsync();
        await _db!.InsertAsync(r);
    }

    public async Task DeleteRecordAsync(ServiceRecord r)
    {
        await InitAsync();
        await _db!.DeleteAsync(r);
    }

    // ---------- Service rules (intervals) ----------
    public async Task<List<ServiceRule>> GetRulesAsync()
    {
        await InitAsync();
        var saved = await _db!.Table<ServiceRule>().ToListAsync();

        foreach (var def in DefaultRules)
        {
            if (saved.All(s => s.ServiceType != def.ServiceType))
            {
                await _db.InsertOrReplaceAsync(def);
                saved.Add(new ServiceRule
                {
                    ServiceType = def.ServiceType,
                    IntervalKm = def.IntervalKm,
                    IntervalDays = def.IntervalDays
                });
            }
        }

        // only the current service types, in the default order
        return DefaultRules
            .Select(d => saved.First(s => s.ServiceType == d.ServiceType))
            .ToList();
    }

    public async Task SaveRuleAsync(ServiceRule rule)
    {
        await InitAsync();
        await _db!.InsertOrReplaceAsync(rule);
    }

    public async Task ResetRulesAsync()
    {
        await InitAsync();
        await _db!.DeleteAllAsync<ServiceRule>();
        await GetRulesAsync();
    }
}
