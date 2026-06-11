using HRMS.API.Models.DTOs.EmployeeAddress;
using HRMS.API.Models.DTOs.EmployeeDocument;
using HRMS.API.Models.DTOs.EmployeeEducation;
using HRMS.API.Models.DTOs.EmployeeEmergencyContact;
using HRMS.API.Models.DTOs.EmployeeExperience;

namespace HRMS.API.Models.DTOs.Employee;

public class EmployeeFullProfileDto
{
    public Guid Id { get; set; }

    public string EmployeeCode { get; set; }
        = string.Empty;

    public string FirstName { get; set; }
        = string.Empty;

    public string LastName { get; set; }
        = string.Empty;

    public string Email { get; set; }
        = string.Empty;

    public string? Phone { get; set; }

    public string? Designation { get; set; }

    public string? Department { get; set; }

    public string? ManagerName { get; set; }

    public string? EmploymentStatus { get; set; }

    public decimal? Salary { get; set; }

    public List<EmployeeEducationResponseDto>
        Educations { get; set; } = [];

    public List<EmployeeExperienceResponseDto>
        Experiences { get; set; } = [];

    public List<EmployeeEmergencyContactResponseDto>
        EmergencyContacts { get; set; } = [];

    public List<EmployeeAddressResponseDto>
        Addresses { get; set; } = [];

    public List<EmployeeDocumentResponseDto>
        Documents { get; set; } = [];
}