using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models.DTOs.Leave;

public class ApplyLeaveDto
{
    public Guid? EmployeeId { get; set; }

    [Required]
    public Guid LeaveTypeId { get; set; }

    [Required]
    public DateOnly FromDate { get; set; }

    [Required]
    public DateOnly ToDate { get; set; }

    [Required]
    [StringLength(500)]
    public string? Reason { get; set; }
}