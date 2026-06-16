using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.EmployeeAddress;

namespace HRMS.API.Interfaces;

public interface IEmployeeAddressService
{
    Task<EmployeeAddressResponseDto> AddAddressAsync(AddEmployeeAddressDto dto, CancellationToken cancellationToken = default);

    Task<PagedResponse<EmployeeAddressResponseDto>> GetAddressesAsync(EmployeeAddressFilterDto filter, CancellationToken cancellationToken = default);

    Task<EmployeeAddressResponseDto> UpdateAddressAsync(Guid addressId, UpdateEmployeeAddressDto dto, CancellationToken cancellationToken = default);

    Task DeleteAddressAsync(Guid addressId, CancellationToken cancellationToken = default);

    Task<byte[]> ExportAddressesAsync(EmployeeAddressFilterDto filter,CancellationToken cancellationToken = default);

    Task<EmployeeAddressImportResultDto> ImportAddressesAsync(IFormFile file,CancellationToken cancellationToken = default);
}