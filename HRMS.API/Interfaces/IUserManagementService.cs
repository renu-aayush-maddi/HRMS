using HRMS.API.Models.DTOs.User;

public interface IUserManagementService
{
    List<UserResponseDto> GetAllUsers();

    UserResponseDto? GetUser(Guid id);

    void DisableUser(Guid id);

    void ActivateUser(Guid id);

    ResetPasswordResponseDto ResetPassword(Guid id);

    void ChangeRole(Guid id,ChangeRoleDto dto);
}