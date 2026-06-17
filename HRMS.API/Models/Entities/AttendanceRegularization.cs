using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class AttendanceRegularization
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public DateTime? RequestedCheckIn { get; set; }

    public DateTime? RequestedCheckOut { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public string? HrComments { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid? ReviewedBy { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Employee? ReviewedByNavigation { get; set; }
}
