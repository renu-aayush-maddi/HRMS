namespace HRMS.API.Models.DTOs.Resignation;

public class EmployeeResignationDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string EmployeeCode { get; set; } = string.Empty;

    public string EmployeeName { get; set; } = string.Empty;

    public DateOnly ResignationDate { get; set; }

    public DateOnly? LastWorkingDate { get; set; }

    public string? Reason { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? HrComments { get; set; }

    public string FinalSettlementStatus { get; set; } = string.Empty;

    public Guid? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public Guid? RejectedBy { get; set; }

    public DateTime? RejectedAt { get; set; }

    public DateTime? WithdrawnAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}