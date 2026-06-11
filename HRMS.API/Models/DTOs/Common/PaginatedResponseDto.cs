namespace HRMS.API.Models.DTOs.Common;

public class PaginatedResponseDto<T>
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalRecords { get; set; }

    public List<T> Data { get; set; }
        = new();
}