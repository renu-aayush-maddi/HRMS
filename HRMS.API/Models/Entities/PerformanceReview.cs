using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class PerformanceReview
{
    public Guid Id { get; set; }

    public Guid? EmployeeId { get; set; }

    public Guid? ReviewerId { get; set; }

    public decimal? Rating { get; set; }

    public string? Comments { get; set; }

    public DateOnly? ReviewDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid PerformanceCycleId { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual PerformanceCycle PerformanceCycle { get; set; } = null!;

    public virtual Employee? Reviewer { get; set; }
}
