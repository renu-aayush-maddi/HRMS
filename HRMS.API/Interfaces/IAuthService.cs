using HRMS.API.Models.DTOs.Auth;

namespace HRMS.API.Interfaces;

public interface IAuthService
{
    Task<string> Register(RegisterDto dto);

    Task<LoginResponseDto> Login(LoginDto dto);
}