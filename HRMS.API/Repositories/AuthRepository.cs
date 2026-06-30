using HRMS.API.Data;
using HRMS.API.Models.Entities;
using HRMS.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Users
            .Include(x => x.Roles)
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User?> GetUserById(Guid id)
    {
        return await _context.Users
            .Include(x => x.Roles)
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Role?> GetRoleByName(string roleName)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(x => x.Name == roleName);
    }

    public async Task AddUser(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task AddEmployee(Employee employee)
    {
        await _context.Employees.AddAsync(employee);
    }

    public async Task AddPasswordResetToken(PasswordResetToken token)
    {
        await _context.PasswordResetTokens.AddAsync(token);
    }

    public async Task<PasswordResetToken?> GetPasswordResetToken(string token)
    {
        return await _context.PasswordResetTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}