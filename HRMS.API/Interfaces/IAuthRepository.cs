using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IAuthRepository
{
    Task<User?> GetUserByEmail(string email);

    Task<Role?> GetRoleByName(string roleName);

    Task AddUser(User user);

    Task AddEmployee(Employee employee);

    Task SaveChanges();
}