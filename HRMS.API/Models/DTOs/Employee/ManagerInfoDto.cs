using System;

namespace HRMS.API.Models.DTOs.Employee;

public class ManagerInfoDto
{
    public Guid Id { get; set; }
    
    public string EmployeeCode { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    
    public string? Designation { get; set; }
    
    public string? Department { get; set; }
    
    public string? ProfilePhotoUrl { get; set; }
    
    public string? EmploymentStatus { get; set; }
}
