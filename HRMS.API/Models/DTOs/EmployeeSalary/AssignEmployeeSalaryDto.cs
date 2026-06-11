namespace HRMS.API.Models.DTOs.EmployeeSalary;

public class AssignEmployeeSalaryDto
{
    public Guid EmployeeId { get; set; }

    public Guid SalaryStructureId { get; set; }

    public decimal AnnualCtc { get; set; }

    public DateOnly EffectiveFrom { get; set; }
}