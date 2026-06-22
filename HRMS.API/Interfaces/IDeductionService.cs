using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.Deduction;

namespace HRMS.API.Interfaces;

public interface IDeductionService
{
    Task<DeductionResponseDto> CreateDeductionAsync(CreateDeductionDto dto, CancellationToken cancellationToken = default);

    Task ApproveDeductionAsync(Guid deductionId, CancellationToken cancellationToken = default);

    Task RejectDeductionAsync(Guid deductionId, CancellationToken cancellationToken = default);

    Task<PagedResponse<DeductionResponseDto>> GetDeductionsAsync(DeductionFilterDto filter, CancellationToken cancellationToken = default);

    Task<PagedResponse<DeductionResponseDto>> GetMyDeductionsAsync(DeductionFilterDto filter, CancellationToken cancellationToken = default);

    Task<DeductionResponseDto> GetDeductionAsync(Guid deductionId, CancellationToken cancellationToken = default);

    Task<byte[]> ExportDeductionsAsync(DeductionFilterDto filter, CancellationToken cancellationToken = default);
}