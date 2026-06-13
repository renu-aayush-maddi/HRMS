using HRMS.API.Models.DTOs.PerformanceBonusRecommendation;

namespace HRMS.API.Interfaces;

public interface IPerformanceBonusRecommendationService
{
    void GenerateRecommendations( Guid cycleId);

    List<
        PerformanceBonusRecommendationResponseDto>
        GetRecommendations();

    void UpdateRecommendation(
        Guid id,
        UpdatePerformanceBonusRecommendationDto dto);

    void ApproveRecommendation(
        Guid id);

    void RejectRecommendation(
        Guid id);

        PerformanceBonusRecommendationResponseDto GetRecommendation(Guid id);
}