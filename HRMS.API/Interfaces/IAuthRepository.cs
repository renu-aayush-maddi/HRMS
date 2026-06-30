using HRMS.API.Models.Entities;
using System.Threading.Tasks;

namespace HRMS.API.Interfaces;

public interface IAuthRepository
{
    Task<User?> GetUserByEmail(string email);

    Task<User?> GetUserById(Guid id);

    Task<Role?> GetRoleByName(string roleName);

    Task AddUser(User user);

    Task AddEmployee(Employee employee);

    Task AddPasswordResetToken(PasswordResetToken token);

    Task<PasswordResetToken?> GetPasswordResetToken(string token);

    Task SaveChanges();
}