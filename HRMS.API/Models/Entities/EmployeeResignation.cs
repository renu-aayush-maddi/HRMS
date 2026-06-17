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

    public Guid? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public Guid? RejectedBy { get; set; }

    public DateTime? RejectedAt { get; set; }

    public DateTime? WithdrawnAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Employee? ApprovedByNavigation { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Employee? RejectedByNavigation { get; set; }
}
