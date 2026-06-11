using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IBonusRepository
{
    Employee? GetEmployee(Guid employeeId);

    Employee? GetEmployeeByUserId(Guid userId);

    Bonuse? GetBonus(Guid bonusId);

    List<Bonuse> GetAllBonuses();

    List<Bonuse> GetEmployeeBonuses(Guid employeeId);

    void AddBonus(Bonuse bonus);

    void UpdateBonus(Bonuse bonus);

    void SaveChanges();
}