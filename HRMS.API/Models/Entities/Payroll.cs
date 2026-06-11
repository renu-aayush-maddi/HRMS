using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class Payroll
{
    public Guid Id { get; set; }

    public Guid? EmployeeId { get; set; }

    public int PayMonth { get; set; }

    public int PayYear { get; set; }

    public decimal BasicSalary { get; set; }

    public decimal? Bonus { get; set; }

    public decimal? Deductions { get; set; }

    public decimal? NetSalary { get; set; }

    public DateTime? GeneratedAt { get; set; }

    public int? WorkingDays { get; set; }

    public int? PresentDays { get; set; }

    public int? LopDays { get; set; }

    public decimal? LopDeduction { get; set; }

    public string? Status { get; set; }

    public decimal? BasicComponent { get; set; }

    public decimal? HraComponent { get; set; }

    public decimal? SpecialAllowanceComponent { get; set; }

    public decimal? MedicalAllowanceComponent { get; set; }

    public decimal? TravelAllowanceComponent { get; set; }

    public virtual Employee? Employee { get; set; }
}
