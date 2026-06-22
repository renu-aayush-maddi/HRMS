using HRMS.API.Models.Common;

namespace HRMS.API.Models.DTOs.Employee;

public class EmployeeFilterDto : PaginationRequestDto
{
    public string? Search { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? ManagerId { get; set; }

    public string? EmploymentStatus { get; set; }

    public string? Designation { get; set; }

    public string? SortBy { get; set; } = "FirstName";

    public bool Descending { get; set; }
}