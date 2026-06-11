namespace HRMS.API.Models.DTOs.Common;

public class PaginationRequestDto
{
    private const int MaxPageSize = 100;

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public int GetValidatedPage()
    {
        return Page < 1 ? 1 : Page;
    }

    public int GetValidatedPageSize()
    {
        if (PageSize < 1)
        {
            return 10;
        }

        return PageSize > MaxPageSize
            ? MaxPageSize
            : PageSize;
    }
}