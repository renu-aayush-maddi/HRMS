namespace HRMS.API.Models.DTOs.Resignation;

public class ResignationResponseDto
{
    public Guid Id { get; set; }

    public string EmployeeName { get; set; }
        = string.Empty;

    public DateOnly ResignationDate { get; set; }

    public DateOnly? LastWorkingDate { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public string? HrComments { get; set; }

    public string? FinalSettlementStatus { get; set; }
}