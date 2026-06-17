namespace HRMS.API.Models.DTOs.Resignation;

public class ResignationExportDto
{
    public string EmployeeCode { get; set; } = string.Empty;

    public string EmployeeName { get; set; } = string.Empty;

    public DateOnly ResignationDate { get; set; }

    public DateOnly? LastWorkingDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string FinalSettlementStatus { get; set; } = string.Empty;

    public string? HrComments { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime? RejectedAt { get; set; }
}