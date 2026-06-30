using HRMS.API.Models.DTOs.Auth;
using System;
using System.Threading.Tasks;

namespace HRMS.API.Interfaces;

public interface IAuthService
{
    Task<string> Register(RegisterDto dto);

    Task<LoginResponseDto> Login(LoginDto dto);

    Task ForgotPassword(ForgotPasswordDto dto, string origin);

    Task<bool> ValidateResetToken(string token);

    Task ResetPassword(ResetPasswordDto dto);

    Task ChangePassword(Guid userId, ChangePasswordDto dto);
}