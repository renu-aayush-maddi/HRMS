namespace HRMS.API.Models.DTOs.EmployeeSalary;

public class EmployeeSalaryResponseDto
{
    public Guid Id { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string SalaryStructureName { get; set; } = string.Empty;

    public decimal AnnualCtc { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public bool? IsActive { get; set; }
}