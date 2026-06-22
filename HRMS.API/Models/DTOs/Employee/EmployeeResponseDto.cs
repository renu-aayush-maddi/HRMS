namespace HRMS.API.Models.DTOs.Employee;

public class EmployeeResponseDto
{
    public Guid Id { get; set; }

    public string EmployeeCode { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string? Phone { get; set; }

    public string? Designation { get; set; }

    public string Department { get; set; }

    public decimal? Salary { get; set; }

    public string? EmploymentStatus { get; set; }

    public Guid? ManagerId { get; set; }

    public string? ManagerName { get; set; }
}