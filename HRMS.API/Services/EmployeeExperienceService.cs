using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.EmployeeExperience;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

public class EmployeeExperienceService
    : IEmployeeExperienceService
{
    private readonly IEmployeeExperienceRepository repository;

    public EmployeeExperienceService(
        IEmployeeExperienceRepository repository)
    {
        this.repository = repository;
    }

    public void AddExperience(
        AddEmployeeExperienceDto dto)
    {
        var employee =
            repository.GetEmployee(
                dto.EmployeeId);

        if (employee == null)
        {
            throw new Exception(
                "Employee not found");
        }

        EmployeeExperience experience = new()
        {
            Id = Guid.NewGuid(),

            EmployeeId =
                dto.EmployeeId,

            CompanyName =
                dto.CompanyName,

            Designation =
                dto.Designation,

            StartDate =
                dto.StartDate,

            EndDate =
                dto.EndDate,

            Responsibilities =
                dto.Responsibilities
        };

        repository.AddExperience(
            experience);

        repository.SaveChanges();
    }

    public List<EmployeeExperienceResponseDto>
        GetEmployeeExperiences(
            Guid employeeId)
    {
        return repository
            .GetEmployeeExperiences(
                employeeId)
            .Select(e =>
                new EmployeeExperienceResponseDto
                {
                    Id = e.Id,

                    CompanyName =
                        e.CompanyName ?? "",

                    Designation =
                        e.Designation ?? "",

                    StartDate =
                        e.StartDate,

                    EndDate =
                        e.EndDate,

                    Responsibilities =
                        e.Responsibilities
                })
            .ToList();
    }

    public void UpdateExperience(
        Guid id,
        UpdateEmployeeExperienceDto dto)
    {
        var experience =
            repository.GetExperience(id);

        if (experience == null)
        {
            throw new Exception(
                "Experience not found");
        }

        experience.CompanyName =
            dto.CompanyName;

        experience.Designation =
            dto.Designation;

        experience.StartDate =
            dto.StartDate;

        experience.EndDate =
            dto.EndDate;

        experience.Responsibilities =
            dto.Responsibilities;

        repository.UpdateExperience(
            experience);

        repository.SaveChanges();
    }

    public void DeleteExperience(
        Guid id)
    {
        var experience =
            repository.GetExperience(id);

        if (experience == null)
        {
            throw new Exception(
                "Experience not found");
        }

        repository.DeleteExperience(
            experience);

        repository.SaveChanges();
    }
}