using BCrypt.Net;
using HRMS.API.Helpers;
using HRMS.API.Models.DTOs.Auth;
using HRMS.API.Models.Entities;
using HRMS.API.Interfaces;
using HRMS.API.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.API.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly JwtHelper _jwtHelper;
    private readonly IEmailService _emailService;

    public AuthService(
        IAuthRepository authRepository, 
        JwtHelper jwtHelper,
        IEmailService emailService)
    {
        _authRepository = authRepository;
        _jwtHelper = jwtHelper;
        _emailService = emailService;
    }

    public async Task<string> Register(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        var existingUser = await _authRepository.GetUserByEmail(email);

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
        var email = dto.Email.Trim().ToLower();

        var user = await _authRepository.GetUserByEmail(email);


        if (user == null)
        {
            throw new BusinessException("Invalid credentials");
        }

        if (user.IsActive == false)
        {
            throw new BusinessException("User account is disabled");
        }
        var validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!validPassword)
        {
            throw new BusinessException("Invalid credentials");
        }

        var role = user.Roles.FirstOrDefault()?.Name;


        if (string.IsNullOrWhiteSpace(role))
        {
            throw new BusinessException("User role not assigned");
        }

        user.LastLoginAt = DateTime.Now;

        await _authRepository.SaveChanges();

        var token = _jwtHelper.GenerateToken(user.Id, user.Email, role, user.Employee?.Id);

        return new LoginResponseDto
        {
            Token = token,

            Email = user.Email,

            Role = role,

            UserId = user.Id,

            EmployeeId = user.Employee?.Id
        };
    }

    private string GenerateEmployeeCode()
    {
        return "EMP" +
               new Random().Next(1000, 9999);
    }

    public async Task ForgotPassword(ForgotPasswordDto dto, string origin)
    {
        var email = dto.Email.Trim().ToLower();
        var user = await _authRepository.GetUserByEmail(email);

        // Do not expose whether user exists (as requested)
        if (user == null)
        {
            return;
        }

        // Generate a secure random token
        var rawToken = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
        
        var resetToken = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = rawToken,
            ExpiresAt = DateTime.Now.AddMinutes(15), // 15 minutes expiration
            IsUsed = false,
            CreatedAt = DateTime.Now
        };

        await _authRepository.AddPasswordResetToken(resetToken);
        await _authRepository.SaveChanges();

        // Construct reset link
        var resetLink = $"{origin}/reset-password?token={rawToken}";

        // Send email with professional styling matching registration flow
        var name = user.Employee != null ? $"{user.Employee.FirstName} {user.Employee.LastName}" : user.Email;
        var emailBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e2e8f0; border-radius: 8px;'>
                <h2 style='color: #2563eb;'>HRMS Password Reset</h2>
                <p>Dear {name},</p>
                <p>We received a request to reset the password associated with your account.</p>
                <p>Please click the button below to set a new password. This link is valid for 15 minutes.</p>
                <div style='margin: 24px 0; text-align: center;'>
                    <a href='{resetLink}' style='background-color: #2563eb; color: #ffffff; padding: 12px 24px; border-radius: 6px; text-decoration: none; font-weight: bold; display: inline-block;'>Reset Password</a>
                </div>
                <p style='color: #64748b; font-size: 0.85rem;'>If the button doesn't work, you can copy and paste the following URL into your browser:</p>
                <p style='color: #2563eb; font-size: 0.85rem; word-break: break-all;'>{resetLink}</p>
                <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 24px 0;' />
                <p style='color: #94a3b8; font-size: 0.8rem;'>If you did not request a password reset, please ignore this email or contact your HR administrator if you have concerns.</p>
            </div>";

        await _emailService.SendEmailAsync(user.Email, "Reset Your HRMS Password", emailBody);
    }

    public async Task<bool> ValidateResetToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return false;

        var resetToken = await _authRepository.GetPasswordResetToken(token);
        if (resetToken == null || resetToken.IsUsed || resetToken.ExpiresAt < DateTime.Now)
        {
            return false;
        }

        return true;
    }

    public async Task ResetPassword(ResetPasswordDto dto)
    {
        var resetToken = await _authRepository.GetPasswordResetToken(dto.Token);
        if (resetToken == null || resetToken.IsUsed || resetToken.ExpiresAt < DateTime.Now)
        {
            throw new BusinessException("Invalid or expired password reset token.");
        }

        var user = resetToken.User;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        resetToken.IsUsed = true;

        await _authRepository.SaveChanges();
    }

    public async Task ChangePassword(Guid userId, ChangePasswordDto dto)
    {
        var user = await _authRepository.GetUserById(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        var validPassword = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);
        if (!validPassword)
        {
            throw new BusinessException("Current password is incorrect.");
        }

        if (dto.CurrentPassword == dto.NewPassword)
        {
            throw new BusinessException("New password cannot be the same as current password.");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _authRepository.SaveChanges();
    }
}