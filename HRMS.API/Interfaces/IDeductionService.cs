using HRMS.API.Models.DTOs.Deduction;

namespace HRMS.API.Interfaces;

public interface IDeductionService
{
    void CreateDeduction(
        CreateDeductionDto dto);

    void ApproveDeduction(
        Guid deductionId);

    void RejectDeduction(
        Guid deductionId);

    List<DeductionResponseDto>
        GetAllDeductions();

    List<DeductionResponseDto>
        GetMyDeductions(Guid userId);
}