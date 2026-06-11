namespace HRMS.API.Models.DTOs.Payroll;

public class PayrollResponseDto
{
    public Guid Id { get; set; }

    public string EmployeeName { get; set; }

    public int PayMonth { get; set; }

    public int PayYear { get; set; }

    public decimal BasicSalary { get; set; }

    public decimal? Bonus { get; set; }

    public decimal? Deductions { get; set; }

    public decimal? NetSalary { get; set; }

    public decimal? BasicComponent { get; set; }

    public decimal? HraComponent { get; set; }

    public decimal? SpecialAllowanceComponent { get; set; }

    public decimal? MedicalAllowanceComponent { get; set; }

    public decimal? TravelAllowanceComponent { get; set; }
}