namespace HRMS.API.Models.DTOs.Payroll;

public class GeneratePayrollDto
{
    public Guid EmployeeId { get; set; }

    public int PayMonth { get; set; }

    public int PayYear { get; set; }

    public decimal Bonus { get; set; }

    public decimal Deductions { get; set; }
}