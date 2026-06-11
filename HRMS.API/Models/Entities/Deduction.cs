using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class Deduction
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public decimal Amount { get; set; }

    public string Reason { get; set; } = null!;

    public int DeductionMonth { get; set; }

    public int DeductionYear { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
