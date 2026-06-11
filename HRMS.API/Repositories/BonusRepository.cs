using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class BonusRepository
    : IBonusRepository
{
    private readonly AppDbContext context;

    public BonusRepository(
        AppDbContext context)
    {
        this.context = context;
    }

    public Employee? GetEmployee(
        Guid employeeId)
    {
        return context.Employees
            .FirstOrDefault(x =>
                x.Id == employeeId);
    }

    public Employee? GetEmployeeByUserId(
        Guid userId)
    {
        return context.Employees
            .FirstOrDefault(x =>
                x.UserId == userId);
    }

    public Bonuse? GetBonus(
        Guid bonusId)
    {
        return context.Bonuses
            .Include(x => x.Employee)
            .FirstOrDefault(x =>
                x.Id == bonusId);
    }

    public List<Bonuse> GetAllBonuses()
    {
        return context.Bonuses
            .Include(x => x.Employee)
            .OrderByDescending(x =>
                x.CreatedAt)
            .ToList();
    }

    public List<Bonuse> GetEmployeeBonuses(
        Guid employeeId)
    {
        return context.Bonuses
            .Include(x => x.Employee)
            .Where(x =>
                x.EmployeeId == employeeId)
            .OrderByDescending(x =>
                x.CreatedAt)
            .ToList();
    }

    public void AddBonus(
        Bonuse bonus)
    {
        context.Bonuses.Add(
            bonus);
    }

    public void UpdateBonus(
        Bonuse bonus)
    {
        context.Bonuses.Update(
            bonus);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}