using AutoCare.Models;

namespace AutoCare.Services;

public static class MaintenanceService
{
    public static List<MaintenanceStatus> Calculate(
        int currentOdo, List<ServiceRecord> records, List<ServiceRule> rules)
    {
        var result = new List<MaintenanceStatus>();

        foreach (var rule in rules)
        {
            bool useKm   = rule.IntervalKm > 0;      // 0 = this limit is not used
            bool useDays = rule.IntervalDays > 0;

            // latest date wins; on the same date the higher odometer wins; then the newest entry
            var last = records.Where(r => r.ServiceType == rule.ServiceType)
                              .OrderByDescending(r => r.ServiceDate.Date)
                              .ThenByDescending(r => r.Odometer)
                              .ThenByDescending(r => r.Id)
                              .FirstOrDefault();

            if (last == null || (!useKm && !useDays))
            {
                result.Add(new MaintenanceStatus
                {
                    Name = rule.ServiceType, Status = "No Record", Level = 3,
                    KmText = "No service logged yet",
                    DaysText = "Log a service to start tracking",
                    StatusColor = Colors.Gray
                });
                continue;
            }

            int kmRemaining   = useKm   ? rule.IntervalKm - (currentOdo - last.Odometer) : int.MaxValue;
            int daysRemaining = useDays ? rule.IntervalDays - (int)(DateTime.Today - last.ServiceDate.Date).TotalDays : int.MaxValue;

            double progress = 0;
            if (useKm)   progress = Math.Max(progress, 1.0 - (double)kmRemaining / rule.IntervalKm);
            if (useDays) progress = Math.Max(progress, 1.0 - (double)daysRemaining / rule.IntervalDays);
            progress = Math.Clamp(progress, 0, 1);

            int level; string status; Color color;
            if (kmRemaining <= 0 || daysRemaining <= 0)
            { level = 0; status = "Overdue";  color = Color.FromArgb("#D93025"); }
            else if ((useKm && kmRemaining <= 1000) || (useDays && daysRemaining <= 30))
            { level = 1; status = "Due Soon"; color = Color.FromArgb("#F29900"); }
            else
            { level = 2; status = "Healthy";  color = Color.FromArgb("#1E8E3E"); }

            string kmPart  = !useKm ? "" : kmRemaining > 0 ? $"{kmRemaining:N0} km" : $"{-kmRemaining:N0} km overdue";
            string dayPart = !useDays ? "" : daysRemaining > 0 ? $"{daysRemaining} days" : $"{-daysRemaining} days overdue";

            string kmText, daysText;
            if (useKm && useDays)
            {
                kmText   = kmRemaining > 0 ? $"{kmRemaining:N0} km remaining" : $"{-kmRemaining:N0} km overdue";
                daysText = daysRemaining > 0 ? $"Approx. {daysRemaining} days left" : $"{-daysRemaining} days overdue";
            }
            else if (useKm)
            {
                kmText   = kmRemaining > 0 ? $"{kmRemaining:N0} km remaining" : $"{-kmRemaining:N0} km overdue";
                daysText = "Distance-based only";
            }
            else
            {
                kmText   = daysRemaining > 0 ? $"{daysRemaining} days remaining" : $"{-daysRemaining} days overdue";
                daysText = "Time-based only (no km limit)";
            }

            result.Add(new MaintenanceStatus
            {
                Name = rule.ServiceType, Status = status, Level = level,
                DueIn = string.Join(" / ", new[] { kmPart, dayPart }.Where(p => p != "")),
                KmText = kmText, DaysText = daysText,
                Progress = progress, StatusColor = color
            });
        }
        return result;
    }
}
