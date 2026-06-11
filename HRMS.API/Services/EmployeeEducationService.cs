using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.EmployeeEducation;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

public class EmployeeEducationService
    : IEmployeeEducationService
{
    private readonly
        IEmployeeEducationRepository repository;

    public EmployeeEducationService(
        IEmployeeEducationRepository repository)
    {
        this.repository = repository;
    }

    public void AddEducation(
        AddEmployeeEducationDto dto)
    {
        var employee =
            repository.GetEmployee(
                dto.EmployeeId);

        if(employee == null)
        {
            throw new Exception(
                "Employee not found");
        }

        EmployeeEducation education =
            new()
            {
                Id = Guid.NewGuid(),

                EmployeeId =
                    dto.EmployeeId,

                Degree =
                    dto.Degree,

                Specialization =
                    dto.Specialization,

                InstitutionName =
                    dto.InstitutionName,

                GraduationYear =
                    dto.GraduationYear,

                Percentage =
                    dto.Percentage
            };

        repository.AddEducation(
            education);

        repository.SaveChanges();
    }

    public List<EmployeeEducationResponseDto>
        GetEmployeeEducations(
            Guid employeeId)
    {
        return repository
            .GetEmployeeEducations(
                employeeId)
            .Select(e =>
                new EmployeeEducationResponseDto
                {
                    Id = e.Id,

                    Degree =
                        e.Degree ?? "",

                    Specialization =
                        e.Specialization ?? "",

                    InstitutionName =
                        e.InstitutionName ?? "",

                    GraduationYear =
                        e.GraduationYear ?? 0,

                    Percentage =
                        e.Percentage
                })
            .ToList();
    }

    public void UpdateEducation(
        Guid id,
        UpdateEmployeeEducationDto dto)
    {
        var education =
            repository.GetEducation(id);

        if(education == null)
        {
            throw new Exception(
                "Education not found");
        }

        education.Degree =
            dto.Degree;

        education.Specialization =
            dto.Specialization;

        education.InstitutionName =
            dto.InstitutionName;

        education.GraduationYear =
            dto.GraduationYear;

        education.Percentage =
            dto.Percentage;

        repository.UpdateEducation(
            education);

        repository.SaveChanges();
    }

    public void DeleteEducation(Guid id)
    {
        var education =
            repository.GetEducation(id);

        if(education == null)
        {
            throw new Exception(
                "Education not found");
        }

        repository.DeleteEducation(
            education);

        repository.SaveChanges();
    }
}