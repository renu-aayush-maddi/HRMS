namespace HRMS.API.Models.DTOs.Resignation;

public class ResignationFilterDto
{
    public Guid? EmployeeId { get; set; }

    public string? Status { get; set; }

    public string? FinalSettlementStatus { get; set; }

    public DateOnly? FromResignationDate { get; set; }

    public DateOnly? ToResignationDate { get; set; }

    public string? SearchTerm { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string SortBy { get; set; } = "CreatedAt";

    public bool SortDescending { get; set; } = true;
}