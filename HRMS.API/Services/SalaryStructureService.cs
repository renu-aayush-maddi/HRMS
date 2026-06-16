using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.SalaryStructure;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

public class SalaryStructureService: ISalaryStructureService
{
    private readonly ISalaryStructureRepository salaryStructureRepository;

    public SalaryStructureService(
        ISalaryStructureRepository salaryStructureRepository)
    {
        this.salaryStructureRepository =
            salaryStructureRepository;
    }

    public void Create(
        CreateSalaryStructureDto dto)
    {
        ValidatePercentages(
            dto.BasicPercentage,
            dto.HraPercentage,
            dto.SpecialAllowancePercentage,
            dto.MedicalAllowancePercentage,
            dto.TravelAllowancePercentage);

        if (salaryStructureRepository
            .GetByName(dto.Name) != null)
        {
            throw new BusinessException(
                "Salary structure already exists");
        }

        SalaryStructure structure =
            new SalaryStructure
            {
                Id = Guid.NewGuid(),

                Name = dto.Name,

                BasicPercentage =
                    dto.BasicPercentage,

                HraPercentage =
                    dto.HraPercentage,

                SpecialAllowancePercentage =
                    dto.SpecialAllowancePercentage,

                MedicalAllowancePercentage =
                    dto.MedicalAllowancePercentage,

                TravelAllowancePercentage =
                    dto.TravelAllowancePercentage
            };

        salaryStructureRepository.Add(
            structure);

        salaryStructureRepository.SaveChanges();
    }

    public void Update(
        Guid id,
        UpdateSalaryStructureDto dto)
    {
        var structure =
            salaryStructureRepository
            .GetById(id);

        if (structure == null)
        {
            throw new NotFoundException(
                "Salary structure not found");
        }

        ValidatePercentages(
            dto.BasicPercentage,
            dto.HraPercentage,
            dto.SpecialAllowancePercentage,
            dto.MedicalAllowancePercentage,
            dto.TravelAllowancePercentage);

        structure.Name =
            dto.Name;

        structure.BasicPercentage =
            dto.BasicPercentage;

        structure.HraPercentage =
            dto.HraPercentage;

        structure.SpecialAllowancePercentage =
            dto.SpecialAllowancePercentage;

        structure.MedicalAllowancePercentage =
            dto.MedicalAllowancePercentage;

        structure.TravelAllowancePercentage =
            dto.TravelAllowancePercentage;

        salaryStructureRepository.Update(
            structure);

        salaryStructureRepository.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var structure =
            salaryStructureRepository
            .GetById(id);

        if (structure == null)
        {
            throw new NotFoundException(
                "Salary structure not found");
        }

        salaryStructureRepository.Delete(
            structure);

        salaryStructureRepository.SaveChanges();
    }

    public SalaryStructureResponseDto GetById(
        Guid id)
    {
        var structure =
            salaryStructureRepository
            .GetById(id);

        if (structure == null)
        {
            throw new NotFoundException(
                "Salary structure not found");
        }

        return Map(structure);
    }

    public List<SalaryStructureResponseDto>
        GetAll()
    {
        return salaryStructureRepository
            .GetAll()
            .Select(Map)
            .ToList();
    }

    private static SalaryStructureResponseDto Map(
        SalaryStructure structure)
    {
        return new SalaryStructureResponseDto
        {
            Id = structure.Id,

            Name = structure.Name,

            BasicPercentage =
                structure.BasicPercentage,

            HraPercentage =
                structure.HraPercentage,

            SpecialAllowancePercentage =
                structure.SpecialAllowancePercentage,

            MedicalAllowancePercentage =
                structure.MedicalAllowancePercentage,

            TravelAllowancePercentage =
                structure.TravelAllowancePercentage
        };
    }

    private static void ValidatePercentages(
        decimal basic,
        decimal hra,
        decimal special,
        decimal medical,
        decimal travel)
    {
        decimal total =
            basic +
            hra +
            special +
            medical +
            travel;

        if (total != 100)
        {
            throw new BusinessException(
                "Salary structure percentages must total 100");
        }
    }
}