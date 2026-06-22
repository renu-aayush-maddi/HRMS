using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.Bonus;

namespace HRMS.API.Interfaces;

public interface IBonusService
{
    Task<BonusResponseDto> CreateBonusAsync(CreateBonusDto dto, CancellationToken cancellationToken = default);

    Task ApproveBonusAsync(Guid bonusId, CancellationToken cancellationToken = default);

    Task RejectBonusAsync(Guid bonusId, CancellationToken cancellationToken = default);

    Task<PagedResponse<BonusResponseDto>> GetBonusesAsync(BonusFilterDto filter, CancellationToken cancellationToken = default);

    Task<PagedResponse<BonusResponseDto>> GetMyBonusesAsync(BonusFilterDto filter, CancellationToken cancellationToken = default);

    Task<BonusResponseDto> GetBonusAsync(Guid bonusId, CancellationToken cancellationToken = default);

    Task<byte[]> ExportBonusesAsync(BonusFilterDto filter, CancellationToken cancellationToken = default);
}