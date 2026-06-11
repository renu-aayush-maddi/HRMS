using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;

namespace HRMS.API.Repositories;

public class SalaryStructureRepository
    : ISalaryStructureRepository
{
    private readonly AppDbContext context;

    public SalaryStructureRepository(
        AppDbContext context)
    {
        this.context = context;
    }

    public void Add(
        SalaryStructure structure)
    {
        context.SalaryStructures.Add(
            structure);
    }

    public void Update(
        SalaryStructure structure)
    {
        context.SalaryStructures.Update(
            structure);
    }

    public void Delete(
        SalaryStructure structure)
    {
        context.SalaryStructures.Remove(
            structure);
    }

    public SalaryStructure? GetById(
        Guid id)
    {
        return context.SalaryStructures
            .FirstOrDefault(x =>
                x.Id == id);
    }

    public SalaryStructure? GetByName(
        string name)
    {
        return context.SalaryStructures
            .FirstOrDefault(x =>
                x.Name == name);
    }

    public List<SalaryStructure> GetAll()
    {
        return context.SalaryStructures
            .OrderBy(x => x.Name)
            .ToList();
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}