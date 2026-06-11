using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class EmployeeAddress
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public string? PostalCode { get; set; }

    public string? AddressType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
