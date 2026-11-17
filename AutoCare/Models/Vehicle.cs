using SQLite;

namespace AutoCare.Models;

public class Vehicle
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string MakeModel { get; set; } = "";
    public string PlateNumber { get; set; } = "";
    public int Year { get; set; }
    public int CurrentOdometer { get; set; }
}