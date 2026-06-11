using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class AttendanceLog
{
    public Guid Id { get; set; }

    public Guid? EmployeeId { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public DateTime? CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee? Employee { get; set; }
}
