using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class LeaveRequest
{
    public Guid Id { get; set; }

    public Guid? EmployeeId { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public string? ManagerComments { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid? LeaveTypeId { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual LeaveType? LeaveType { get; set; }
}
