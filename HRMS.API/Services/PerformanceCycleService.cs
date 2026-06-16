using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.PerformanceCycle;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

public class PerformanceCycleService : IPerformanceCycleService
{
    private readonly IPerformanceCycleRepository repository;

    public PerformanceCycleService(IPerformanceCycleRepository repository)
    {
        this.repository = repository;
    }

    public void AddCycle(AddPerformanceCycleDto dto)
    {
        if (dto.StartDate >= dto.EndDate)
        {
            throw new BusinessException("StartDate must be before EndDate");
        }

        if (repository.CycleNameExists(dto.Name))
        {
            throw new BusinessException("Cycle name already exists");
        }

        repository.Add(new PerformanceCycle
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = "Open",
            CreatedAt = DateTime.Now
        });

        repository.SaveChanges();
    }

    public void UpdateCycle(Guid id, UpdatePerformanceCycleDto dto)
    {
        var cycle = repository.GetById(id);

        if (cycle == null)
        {
            throw new NotFoundException("Cycle not found");
        }

        if (dto.StartDate >= dto.EndDate)
        {
            throw new BusinessException("StartDate must be before EndDate");
        }

        if (repository.CycleNameExists(dto.Name, id))
        {
            throw new BusinessException("Cycle name already exists");
        }

        cycle.Name = dto.Name;
        cycle.StartDate = dto.StartDate;
        cycle.EndDate = dto.EndDate;
        cycle.Status = dto.Status;

        repository.Update(cycle);
        repository.SaveChanges();
    }

    public void DeleteCycle(Guid id)
    {
        var cycle = repository.GetById(id);

        if (cycle == null)
        {
            throw new NotFoundException("Cycle not found");
        }

        if (repository.HasReviews(id))
        {
            throw new BusinessException("Cycle contains reviews");
        }

        if (repository.HasRecommendations(id))
        {
            throw new BusinessException("Cycle contains recommendations");
        }

        repository.Delete(cycle);
        repository.SaveChanges();
    }

    public List<PerformanceCycleResponseDto> GetAllCycles()
    {
        return repository.GetAll()
            .Select(x => new PerformanceCycleResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Status = x.Status
            })
            .ToList();
    }

    public PerformanceCycleResponseDto GetCycleById(Guid id)
    {
        var cycle = repository.GetById(id);

        if (cycle == null)
        {
            throw new NotFoundException("Cycle not found");
        }

        return new PerformanceCycleResponseDto
        {
            Id = cycle.Id,
            Name = cycle.Name,
            StartDate = cycle.StartDate,
            EndDate = cycle.EndDate,
            Status = cycle.Status
        };
    }
}