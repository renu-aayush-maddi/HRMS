namespace HRMS.API.Models.Common;

public class PaginationRequestDto
{
    private const int MaxPageSize = 100;

    public int PageNumber { get; set; } = 1;

    private int pageSize = 10;

    public int PageSize
    {
        get => pageSize;

        set => pageSize =
            value > MaxPageSize
                ? MaxPageSize
                : value;
    }
}