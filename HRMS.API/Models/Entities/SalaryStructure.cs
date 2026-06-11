using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class SalaryStructure
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal BasicPercentage { get; set; }

    public decimal HraPercentage { get; set; }

    public decimal SpecialAllowancePercentage { get; set; }

    public decimal MedicalAllowancePercentage { get; set; }

    public decimal TravelAllowancePercentage { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<EmployeeSalary> EmployeeSalaries { get; set; } = new List<EmployeeSalary>();
}
