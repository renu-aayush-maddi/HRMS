using HRMS.API.Models.Common;

namespace HRMS.API.Models.DTOs.EmployeeAddress;

public class EmployeeAddressFilterDto: PaginationRequestDto
{
    public Guid? EmployeeId { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public string? AddressType { get; set; }

    public string? SortBy { get; set; }

    public bool Descending { get; set; }
}