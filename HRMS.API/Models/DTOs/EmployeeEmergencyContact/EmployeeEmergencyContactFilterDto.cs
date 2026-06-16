using HRMS.API.Models.Common;

namespace HRMS.API.Models.DTOs.EmployeeEmergencyContact;

public class EmployeeEmergencyContactFilterDto : PaginationRequestDto
{
    public Guid? EmployeeId { get; set; }

    public string? ContactName { get; set; }

    public string? Relationship { get; set; }

    public string? SortBy { get; set; }

    public bool Descending { get; set; }
}