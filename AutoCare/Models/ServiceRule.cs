using SQLite;

namespace AutoCare.Models;

public class ServiceRule
{
    [PrimaryKey]
    public string ServiceType { get; set; } = "";   // e.g. "Engine Oil"
    public int IntervalKm { get; set; }             // service every X km
    public int IntervalDays { get; set; }           // or every X days
}