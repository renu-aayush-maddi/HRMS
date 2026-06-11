namespace HRMS.API.Models.DTOs.Dashboard;

public class PayrollSummaryDto
{
    public decimal TotalPayroll { get; set; }

    public decimal AverageSalary { get; set; }

    public decimal HighestSalary { get; set; }

    public int EmployeesPaid { get; set; }
}