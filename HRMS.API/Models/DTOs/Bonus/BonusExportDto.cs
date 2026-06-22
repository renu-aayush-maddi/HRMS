namespace HRMS.API.Models.DTOs.Bonus;

public class BonusExportDto
{
    public string EmployeeName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Reason { get; set; } = string.Empty;

    public int BonusMonth { get; set; }

    public int BonusYear { get; set; }

    public string Status { get; set; } = string.Empty;
}