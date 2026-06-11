namespace HRMS.API.Models.DTOs.Employee;

public class AddEmployeeDto
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string? Phone { get; set; }

    public string? Designation { get; set; }

    public Guid DepartmentId { get; set; }

    public decimal Salary { get; set; }

    public string Role { get; set; }
}