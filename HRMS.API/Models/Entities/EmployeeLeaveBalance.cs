using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class EmployeeLeaveBalance
{
    public Guid Id { get; set; }

    public Guid? EmployeeId { get; set; }

    public Guid? LeaveTypeId { get; set; }

    public int AllocatedDays { get; set; }

    public int? UsedDays { get; set; }

    public int RemainingDays { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual LeaveType? LeaveType { get; set; }
}
