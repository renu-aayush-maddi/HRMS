namespace HRMS.API.Models.DTOs.Deduction;

public class CreateDeductionDto
{
    public Guid EmployeeId { get; set; }

    public decimal Amount { get; set; }

    public string Reason { get; set; } = string.Empty;

    public int DeductionMonth { get; set; }

    public int DeductionYear { get; set; }
}