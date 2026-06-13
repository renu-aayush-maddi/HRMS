using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class PerformanceBonusRule
{
    public Guid Id { get; set; }

    public decimal MinRating { get; set; }

    public decimal MaxRating { get; set; }

    public decimal BonusPercentage { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }
}
