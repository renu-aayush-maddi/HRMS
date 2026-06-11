using HRMS.API.Models.DTOs.Resignation;

namespace HRMS.API.Interfaces;

public interface IResignationService
{
    void SubmitResignation(
        SubmitResignationDto dto);

    List<ResignationResponseDto>
        GetAll();

    List<ResignationResponseDto>
        GetEmployeeResignations(
            Guid employeeId);

    void Approve(
        Guid resignationId,
        ResignationActionDto dto);

    void Reject(
        Guid resignationId,
        ResignationActionDto dto);

    void UpdateSettlement(
        Guid resignationId,
        SettlementDto dto);
}