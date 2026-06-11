using BCrypt.Net;
using HRMS.API.Helpers;
using HRMS.API.Models.DTOs.Auth;
using HRMS.API.Models.Entities;
using HRMS.API.Interfaces;
using HRMS.API.Exceptions;

namespace HRMS.API.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;

    private readonly JwtHelper _jwtHelper;

    public AuthService(IAuthRepository authRepository,JwtHelper jwtHelper)
    {
        _authRepository = authRepository;

        _jwtHelper = jwtHelper;
    }

    public async Task<string> Register(RegisterDto dto)
    {
        var existingUser = await _authRepository.GetUserByEmail(dto.Email);

        if (existingUser != null)
        {
            throw new BusinessException("User already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),

            Email = dto.Email,

            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),

            IsActive = true
        };

        await _authRepository.AddUser(user);

        var role = await _authRepository.GetRoleByName(dto.Role);

        if (role == null)
        {
            throw new BusinessException("Invalid role");
        }

        user.Roles.Add(role);

        var employee = new Employee
        {
            Id = Guid.NewGuid(),

            UserId = user.Id,

            EmployeeCode = GenerateEmployeeCode(),

            FirstName = dto.FirstName,

            LastName = dto.LastName,

            Email = dto.Email,

            DepartmentId = dto.DepartmentId,

            Designation = dto.Designation,

            Salary = dto.Salary,

            JoiningDate = DateOnly.FromDateTime(DateTime.UtcNow),

            EmploymentStatus = "Active"
        };

        await _authRepository.AddEmployee(employee);

        await _authRepository.SaveChanges();

        return "User Registered Successfully";
    }

    public async Task<LoginResponseDto> Login(LoginDto dto)
    {
        var user =
            await _authRepository.GetUserByEmail(dto.Email);

        if (user == null)
        {
            throw new BusinessException("Invalid credentials");
        }

        if(user.IsActive == false)
        {
            throw new BusinessException("User account is disabled");
        }

        var validPassword =
            BCrypt.Net.BCrypt.Verify(
                dto.Password,
                user.PasswordHash);

        if (!validPassword)
        {
            throw new BusinessException("Invalid credentials");
        }

        var role =
            await _authRepository.GetUserRole(user.Id);

        var token =
            _jwtHelper.GenerateToken(
                user.Id,
                user.Email,
                role!);

        return new LoginResponseDto
        {
            Token = token,

            Email = user.Email,

            Role = role!
        };
    }

    private string GenerateEmployeeCode()
    {
        return "EMP" +
               new Random().Next(1000, 9999);
    }
}