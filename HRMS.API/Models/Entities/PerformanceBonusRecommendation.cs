using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class PerformanceBonusRecommendation
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public decimal AverageRating { get; set; }

    public decimal RecommendedPercentage { get; set; }

    public decimal RecommendedAmount { get; set; }

    public decimal? ApprovedAmount { get; set; }

    public string? Remarks { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public Guid PerformanceCycleId { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
