using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class Holiday
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly HolidayDate { get; set; }

    public string? Description { get; set; }

    public bool? IsOptional { get; set; }

    public DateTime? CreatedAt { get; set; }
}
