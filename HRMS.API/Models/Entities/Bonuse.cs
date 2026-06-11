using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class Bonuse
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public decimal Amount { get; set; }

    public string Reason { get; set; } = null!;

    public int BonusMonth { get; set; }

    public int BonusYear { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
