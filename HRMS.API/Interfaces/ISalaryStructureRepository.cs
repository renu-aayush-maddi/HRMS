using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface ISalaryStructureRepository
{
    void Add(SalaryStructure structure);

    void Update(SalaryStructure structure);

    void Delete(SalaryStructure structure);

    SalaryStructure? GetById(Guid id);

    SalaryStructure? GetByName(string name);

    List<SalaryStructure> GetAll();

    void SaveChanges();
}