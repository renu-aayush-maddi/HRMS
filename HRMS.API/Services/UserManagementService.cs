using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.User;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

public class UserManagementService : IUserManagementService
{

private readonly IUserManagementRepository repository;

        public UserManagementService(
            IUserManagementRepository repository)
        {
            this.repository = repository;
        }

        public List<UserResponseDto> GetAllUsers()
        {
            return repository
                .GetAllUsers()
                .Select(x => new UserResponseDto
                {
                    Id = x.Id,

                    Email = x.Email,

                    Role =
                        x.Roles.FirstOrDefault()?.Name
                        ?? "",

                    IsActive =
                        x.IsActive ?? false,

                    EmployeeName =
                        x.Employee == null
                        ? ""
                        : x.Employee.FirstName +
                        " " +
                        x.Employee.LastName
                })
                .ToList();
        }

        public UserResponseDto? GetUser(Guid id)
        {
            var user = repository.GetUserById(id);

            if (user == null)
            {
                return null;
            }

            return new UserResponseDto
            {
                Id = user.Id,

                Email = user.Email,

                Role =
                    user.Roles.FirstOrDefault()?.Name
                    ?? "",

                IsActive =
                    user.IsActive ?? false,

                EmployeeName =
                    user.Employee == null
                    ? ""
                    : user.Employee.FirstName +
                    " " +
                    user.Employee.LastName
            };
        }

        public void DisableUser(Guid id)
        {
            var user =
                repository.GetUserById(id);

            if(user == null)
            {
                throw new Exception(
                    "User not found");
            }

            user.IsActive = false;

            repository.UpdateUser(user);

            repository.SaveChanges();
        }

        public void ActivateUser(Guid id)
        {
            var user =
                repository.GetUserById(id);

            if(user == null)
            {
                throw new Exception(
                    "User not found");
            }

            user.IsActive = true;

            repository.UpdateUser(user);

            repository.SaveChanges();
        }

        public ResetPasswordResponseDto
            ResetPassword(Guid id)
        {
            var user =
                repository.GetUserById(id);

            if(user == null)
            {
                throw new Exception(
                    "User not found");
            }

            string password =
                "HRMS@" +
                new Random().Next(1000,9999);

            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    password);

            repository.UpdateUser(user);

            repository.SaveChanges();

            return new ResetPasswordResponseDto
            {
                TemporaryPassword = password
            };
        }

        public void ChangeRole(
            Guid id,
            ChangeRoleDto dto)
        {
            var user =
                repository.GetUserById(id);

            if(user == null)
            {
                throw new Exception(
                    "User not found");
            }

            var role =
                repository.GetRoleByName(dto.Role);

            if(role == null)
            {
                throw new Exception(
                    "Invalid role");
            }

            user.Roles.Clear();

            user.Roles.Add(role);

            repository.UpdateUser(user);

            repository.SaveChanges();
        }

}
