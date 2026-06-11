using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class EmployeeExperience
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string? CompanyName { get; set; }

    public string? Designation { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Responsibilities { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
