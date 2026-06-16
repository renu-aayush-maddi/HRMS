using HRMS.API.Models.Common;

namespace HRMS.API.Models.DTOs.EmployeeEducation;

public class EmployeeEducationFilterDto: PaginationRequestDto
{
    public Guid? EmployeeId { get; set; }

    public string? Degree { get; set; }

    public string? InstitutionName { get; set; }

    public int? GraduationYear { get; set; }

    public string? SortBy { get; set; }

    public bool Descending { get; set; }
}