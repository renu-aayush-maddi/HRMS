using HRMS.API.Models.Common;

namespace HRMS.API.Models.DTOs.Deduction;

public class DeductionFilterDto : PaginationRequestDto
{
    public Guid? EmployeeId { get; set; }

    public string? Status { get; set; }

    public int? DeductionMonth { get; set; }

    public int? DeductionYear { get; set; }

    public decimal? MinAmount { get; set; }

    public decimal? MaxAmount { get; set; }

    public string? SortBy { get; set; }

    public bool Descending { get; set; }
}