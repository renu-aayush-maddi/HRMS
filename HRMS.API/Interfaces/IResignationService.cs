using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.Resignation;

namespace HRMS.API.Interfaces;

public interface IEmployeeResignationService
{
    Task<EmployeeResignationDto> CreateAsync(CreateResignationDto dto, CancellationToken cancellationToken = default);

    Task<EmployeeResignationDto> GetByIdAsync(Guid resignationId, CancellationToken cancellationToken = default);

    Task<PagedResult<EmployeeResignationDto>> GetAllAsync(ResignationFilterDto filter, CancellationToken cancellationToken = default);

    Task ApproveAsync(Guid resignationId, CancellationToken cancellationToken = default);

    Task RejectAsync(Guid resignationId, RejectResignationDto dto, CancellationToken cancellationToken = default);

    Task WithdrawAsync(Guid resignationId, CancellationToken cancellationToken = default);

    Task UpdateSettlementStatusAsync(Guid resignationId, UpdateSettlementStatusDto dto, CancellationToken cancellationToken = default);

    Task<byte[]> ExportAsync(ResignationFilterDto filter, CancellationToken cancellationToken = default);
}