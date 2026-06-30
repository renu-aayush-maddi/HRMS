using System;
using System.Collections.Generic;

namespace HRMS.API.Models.DTOs.Employee;

public class EmployeeNodeDto
{
    public Guid Id { get; set; }
    
    public Guid? ManagerId { get; set; }
    
    public string EmployeeCode { get; set; } = null!;
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public string Name => $"{FirstName} {LastName}";
    
    public string Email { get; set; } = null!;
    
    public string? Designation { get; set; }
    
    public Guid? DepartmentId { get; set; }
    
    public string? DepartmentName { get; set; }
    
    public string? EmploymentStatus { get; set; }
    
    public string? ProfilePhotoUrl { get; set; }
    
    public int DirectReportsCount { get; set; }
    
    public string? Warning { get; set; }

    public List<EmployeeNodeDto> Children { get; set; } = new();
}
