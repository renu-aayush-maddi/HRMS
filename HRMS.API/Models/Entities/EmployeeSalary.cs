using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class EmployeeSalary
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid SalaryStructureId { get; set; }

    public decimal AnnualCtc { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual SalaryStructure SalaryStructure { get; set; } = null!;
}
