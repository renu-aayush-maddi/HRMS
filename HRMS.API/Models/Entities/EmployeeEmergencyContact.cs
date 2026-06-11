using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class EmployeeEmergencyContact
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string ContactName { get; set; } = null!;

    public string? Relationship { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
