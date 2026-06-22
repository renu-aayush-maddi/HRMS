using HRMS.API.Models.Common;

namespace HRMS.API.Models.DTOs.Leave;

public class LeaveFilterDto : PaginationRequestDto
{
    public Guid? EmployeeId { get; set; }

    public Guid? LeaveTypeId { get; set; }

    public string? Status { get; set; }

    public DateOnly? FromDate { get; set; }

    public DateOnly? ToDate { get; set; }

    public string? SortBy { get; set; }

    public bool Descending { get; set; }
}