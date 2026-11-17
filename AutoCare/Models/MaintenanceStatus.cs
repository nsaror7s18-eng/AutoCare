namespace AutoCare.Models;

public class MaintenanceStatus
{
    public string Name { get; set; } = "";
    public string Status { get; set; } = "";
    public int Level { get; set; }            // 0 Overdue, 1 Due Soon, 2 Healthy, 3 No Record
    public string DueIn { get; set; } = "";   // e.g. "800 km / 12 days" or "120 days"
    public string KmText { get; set; } = "";
    public string DaysText { get; set; } = "";
    public double Progress { get; set; }      // 0 = just serviced, 1 = fully used up
    public Color StatusColor { get; set; } = Colors.Gray;
}
