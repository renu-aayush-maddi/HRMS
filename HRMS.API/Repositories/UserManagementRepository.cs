using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

public class UserManagementRepository
    : IUserManagementRepository
{
    private readonly AppDbContext context;

    public UserManagementRepository(
        AppDbContext context)
    {
        this.context = context;
    }

    public List<User> GetAllUsers()
    {
        return context.Users
            .Include(x => x.Roles)
            .Include(x => x.Employee)
            .ToList();
    }

    public User? GetUserById(Guid id)
    {
        return context.Users
            .Include(x => x.Roles)
            .Include(x => x.Employee)
            .FirstOrDefault(x => x.Id == id);
    }

    public Role? GetRoleByName(string roleName)
    {
        return context.Roles
            .FirstOrDefault(x => x.Name == roleName);
    }

    public void UpdateUser(User user)
    {
        context.Users.Update(user);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}