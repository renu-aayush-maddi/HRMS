namespace HRMS.API.Models.DTOs.Deduction;

public class DeductionResponseDto
{
    public Guid Id { get; set; }

    public string EmployeeName { get; set; }
        = string.Empty;

    public decimal Amount { get; set; }

    public string Reason { get; set; }
        = string.Empty;

    public int DeductionMonth { get; set; }

    public int DeductionYear { get; set; }

    public string Status { get; set; }
        = string.Empty;
}