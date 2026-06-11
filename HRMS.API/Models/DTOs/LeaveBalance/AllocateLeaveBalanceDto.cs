using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models.DTOs.LeaveBalance;

public class AllocateLeaveBalanceDto
{
    public Guid EmployeeId { get; set; }

    public Guid LeaveTypeId { get; set; }

    [Range(1, 365)]
    public int AllocatedDays { get; set; }
}