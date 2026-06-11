namespace HRMS.API.Models.DTOs.SalaryStructure;

public class UpdateSalaryStructureDto
{
    public string Name { get; set; } = string.Empty;

    public decimal BasicPercentage { get; set; }

    public decimal HraPercentage { get; set; }

    public decimal SpecialAllowancePercentage { get; set; }

    public decimal MedicalAllowancePercentage { get; set; }

    public decimal TravelAllowancePercentage { get; set; }
}