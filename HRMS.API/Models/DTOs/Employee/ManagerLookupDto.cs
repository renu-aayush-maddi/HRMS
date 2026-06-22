namespace HRMS.API.Models.DTOs.Employee;

public class ManagerLookupDto
{
    public Guid Id { get; set; }

    public string EmployeeCode { get; set; }
        = string.Empty;

    public string FullName { get; set; }
        = string.Empty;

    public string? Designation { get; set; }
}