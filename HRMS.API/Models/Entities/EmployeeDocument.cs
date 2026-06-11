using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class EmployeeDocument
{
    public Guid Id { get; set; }

    public Guid? EmployeeId { get; set; }

    public string DocumentName { get; set; } = null!;

    public string? DocumentType { get; set; }

    public string FileUrl { get; set; } = null!;

    public DateTime? UploadedAt { get; set; }

    public bool? IsVerified { get; set; }

    public Guid? VerifiedBy { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual User? VerifiedByNavigation { get; set; }
}
