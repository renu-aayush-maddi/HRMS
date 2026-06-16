using HRMS.API.Models.Common;

namespace HRMS.API.Models.DTOs.EmployeeDocument;

public class EmployeeDocumentFilterDto: PaginationRequestDto
{
    public Guid? EmployeeId { get; set; }

    public string? DocumentType { get; set; }

    public bool? IsVerified { get; set; }

    public string? SortBy { get; set; }

    public bool Descending { get; set; }
}