using SQLite;

namespace AutoCare.Models;

public class ServiceRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string ServiceType { get; set; } = "";
    public DateTime ServiceDate { get; set; }
    public double Cost { get; set; }
    public int Odometer { get; set; }
    public string Notes { get; set; } = "";
}