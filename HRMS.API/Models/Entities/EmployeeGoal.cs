using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class EmployeeGoal
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid AssignedBy { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? TargetDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee AssignedByNavigation { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;
}
