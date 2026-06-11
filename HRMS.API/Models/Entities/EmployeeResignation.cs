using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class EmployeeResignation
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public DateOnly ResignationDate { get; set; }

    public DateOnly? LastWorkingDate { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public string? HrComments { get; set; }

    public string? FinalSettlementStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
