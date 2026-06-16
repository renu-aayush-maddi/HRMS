using HRMS.API.Models.Common;

namespace HRMS.API.Models.DTOs.EmployeeExperience;

public class EmployeeExperienceFilterDto
    : PaginationRequestDto
{
    public Guid? EmployeeId { get; set; }

    public string? CompanyName { get; set; }

    public string? Designation { get; set; }

    public string? SortBy { get; set; }

    public bool Descending { get; set; }
}