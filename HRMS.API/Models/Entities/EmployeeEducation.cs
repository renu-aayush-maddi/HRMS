using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class EmployeeEducation
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string? Degree { get; set; }

    public string? Specialization { get; set; }

    public string? InstitutionName { get; set; }

    public int? GraduationYear { get; set; }

    public decimal? Percentage { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
