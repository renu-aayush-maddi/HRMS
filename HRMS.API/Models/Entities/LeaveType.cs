using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class LeaveType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int AnnualAllocation { get; set; }

    public bool? CarryForwardAllowed { get; set; }

    public int? MaxCarryForward { get; set; }

    public bool? NegativeBalanceAllowed { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<EmployeeLeaveBalance> EmployeeLeaveBalances { get; set; } = new List<EmployeeLeaveBalance>();

    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
}
