using HRMS.API.Models.Common;

namespace HRMS.API.Models.DTOs.Bonus;

public class BonusFilterDto : PaginationRequestDto
{
    public Guid? EmployeeId { get; set; }

    public string? Status { get; set; }

    public int? BonusMonth { get; set; }

    public int? BonusYear { get; set; }

    public decimal? MinAmount { get; set; }

    public decimal? MaxAmount { get; set; }

    public string? SortBy { get; set; }

    public bool Descending { get; set; }
}