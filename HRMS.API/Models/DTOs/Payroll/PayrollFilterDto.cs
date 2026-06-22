using HRMS.API.Models.Common;

namespace HRMS.API.Models.DTOs.Payroll;

public class PayrollFilterDto : PaginationRequestDto
{
    public Guid? EmployeeId { get; set; }

    public int? PayMonth { get; set; }

    public int? PayYear { get; set; }

    public string? Status { get; set; }

    public decimal? MinNetSalary { get; set; }

    public decimal? MaxNetSalary { get; set; }

    public string? SortBy { get; set; }

    public bool Descending { get; set; }
}