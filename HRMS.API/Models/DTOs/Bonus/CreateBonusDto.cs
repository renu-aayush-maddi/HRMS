namespace HRMS.API.Models.DTOs.Bonus;

public class CreateBonusDto
{
    public Guid EmployeeId { get; set; }

    public decimal Amount { get; set; }

    public string Reason { get; set; } = string.Empty;

    public int BonusMonth { get; set; }

    public int BonusYear { get; set; }
}