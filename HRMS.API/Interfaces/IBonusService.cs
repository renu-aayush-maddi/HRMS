using HRMS.API.Models.DTOs.Bonus;

namespace HRMS.API.Interfaces;

public interface IBonusService
{
    void CreateBonus(
        CreateBonusDto dto);

    void ApproveBonus(
        Guid bonusId);

    void RejectBonus(
        Guid bonusId);

    List<BonusResponseDto>
        GetAllBonuses();

    List<BonusResponseDto>
        GetMyBonuses(Guid userId);
}