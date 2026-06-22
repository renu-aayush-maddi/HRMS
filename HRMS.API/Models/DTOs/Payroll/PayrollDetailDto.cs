namespace HRMS.API.Models.DTOs.Payroll;

public class PayrollDetailDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public int PayMonth { get; set; }

    public int PayYear { get; set; }

    public decimal BasicSalary { get; set; }

    public decimal? Bonus { get; set; }

    public decimal? Deductions { get; set; }

    public decimal? NetSalary { get; set; }

    public int? WorkingDays { get; set; }

    public int? PresentDays { get; set; }

    public int? LopDays { get; set; }

    public decimal? LopDeduction { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal? BasicComponent { get; set; }

    public decimal? HraComponent { get; set; }

    public decimal? SpecialAllowanceComponent { get; set; }

    public decimal? MedicalAllowanceComponent { get; set; }

    public decimal? TravelAllowanceComponent { get; set; }

    public DateTime? GeneratedAt { get; set; }
}