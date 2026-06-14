using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class AuditLog
{
    public Guid Id { get; set; }

    public string Action { get; set; } = null!;

    public string EntityType { get; set; } = null!;

    public Guid EntityId { get; set; }

    public string? Details { get; set; }

    public Guid? PerformedBy { get; set; }

    public DateTime PerformedAt { get; set; }
}
