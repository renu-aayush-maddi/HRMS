using HRMS.API.Models.Entities;

public interface IUserManagementRepository
{
    List<User> GetAllUsers();

    User? GetUserById(Guid id);

    Role? GetRoleByName(string roleName);

    void UpdateUser(User user);

    void SaveChanges();
}