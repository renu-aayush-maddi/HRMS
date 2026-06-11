namespace HRMS.API.Models.DTOs.Employee;

public class UpdateEmployeeDto
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string? Phone { get; set; }

    public string? Designation { get; set; }

    public decimal Salary { get; set; }
}