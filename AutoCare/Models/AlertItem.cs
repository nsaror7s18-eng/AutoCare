namespace AutoCare.Models;

public class AlertItem
{
    public string Badge { get; set; } = "";
    public string Message { get; set; } = "";
    public string Detail { get; set; } = "";
    public Color BadgeColor { get; set; } = Colors.Gray;
    public Color CardColor { get; set; } = Colors.White;
}