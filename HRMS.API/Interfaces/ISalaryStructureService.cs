using HRMS.API.Models.DTOs.SalaryStructure;

namespace HRMS.API.Interfaces;

public interface ISalaryStructureService
{
    void Create(
        CreateSalaryStructureDto dto);

    void Update(
        Guid id,
        UpdateSalaryStructureDto dto);

    void Delete(
        Guid id);

    SalaryStructureResponseDto GetById(
        Guid id);

    List<SalaryStructureResponseDto> GetAll();
}