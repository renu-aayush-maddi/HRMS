using HRMS.API.Models.DTOs.PerformanceCycle;

namespace HRMS.API.Interfaces;

public interface IPerformanceCycleService
{
    void AddCycle(AddPerformanceCycleDto dto);

    void UpdateCycle(Guid id, UpdatePerformanceCycleDto dto);

    void DeleteCycle(Guid id);

    List<PerformanceCycleResponseDto> GetAllCycles();

    PerformanceCycleResponseDto GetCycleById(Guid id);
}