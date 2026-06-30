using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.User;

namespace HRMS.Tests.Services;

[TestFixture]
public class UserManagementServiceTests
{
    private Mock<IUserManagementRepository>
        repository;

    private UserManagementService
        service;

    [SetUp]
    public void Setup()
    {
        repository =
            new Mock<IUserManagementRepository>();

        service =
            new UserManagementService(
                repository.Object);
    }

    [Test]
    public void GetAllUsers_ShouldReturnMappedDtos()
    {
        repository
            .Setup(x =>
                x.GetAllUsers())
            .Returns(
                new List<User>
                {
                    new User
                    {
                        Id = Guid.NewGuid(),

                        Email = "john@test.com",

                        IsActive = true,

                        Roles =
                        [
                            new Role
                            {
                                Name = "Admin"
                            }
                        ],

                        Employee =
                            new Employee
                            {
                                FirstName = "John",

                                LastName = "Doe"
                            }
                    }
                });

        var result =
            service.GetAllUsers();

        Assert.That(
            result.Count,
            Is.EqualTo(1));

        Assert.That(
            result[0].Email,
            Is.EqualTo("john@test.com"));

        Assert.That(
            result[0].Role,
            Is.EqualTo("Admin"));

        Assert.That(
            result[0].EmployeeName,
            Is.EqualTo("John Doe"));
    }

    [Test]
    public void GetAllUsers_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        repository
            .Setup(x =>
                x.GetAllUsers())
            .Returns(
                []);

        var result =
            service.GetAllUsers();

        Assert.That(
            result,
            Is.Empty);
    }

    [Test]
    public void GetUser_ShouldReturnMappedDto()
    {
        var userId =
            Guid.NewGuid();

        repository
            .Setup(x =>
                x.GetUserById(
                    userId))
            .Returns(
                new User
                {
                    Id = userId,

                    Email = "john@test.com",

                    IsActive = true,

                    Roles =
                    [
                        new Role
                        {
                            Name = "Admin"
                        }
                    ],

                    Employee =
                        new Employee
                        {
                            FirstName = "John",

                            LastName = "Doe"
                        }
                });

        var result =
            service.GetUser(
                userId);

        Assert.That(
            result,
            Is.Not.Null);

        Assert.That(
            result!.Role,
            Is.EqualTo("Admin"));

        Assert.That(
            result.EmployeeName,
            Is.EqualTo("John Doe"));
    }

    [Test]
    public void GetUser_ShouldReturnNull_WhenUserNotFound()
    {
        repository
            .Setup(x =>
                x.GetUserById(
                    It.IsAny<Guid>()))
            .Returns(
                (User?)null);

        var result =
            service.GetUser(
                Guid.NewGuid());

        Assert.That(
            result,
            Is.Null);
    }

    [Test]
    public void DisableUser_ShouldDisableUser_WhenUserExists()
    {
        var user =
            new User
            {
                IsActive = true
            };

        repository
            .Setup(x =>
                x.GetUserById(
                    It.IsAny<Guid>()))
            .Returns(user);

        service.DisableUser(
            Guid.NewGuid());

        Assert.That(
            user.IsActive,
            Is.False);

        repository.Verify(
            x => x.UpdateUser(user),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void DisableUser_ShouldThrowException_WhenUserNotFound()
    {
        repository
            .Setup(x =>
                x.GetUserById(
                    It.IsAny<Guid>()))
            .Returns(
                (User?)null);

        Assert.Throws<Exception>(
            () =>
                service.DisableUser(
                    Guid.NewGuid()));
    }

    [Test]
    public void ActivateUser_ShouldActivateUser_WhenUserExists()
    {
        var user =
            new User
            {
                IsActive = false
            };

        repository
            .Setup(x =>
                x.GetUserById(
                    It.IsAny<Guid>()))
            .Returns(user);

        service.ActivateUser(
            Guid.NewGuid());

        Assert.That(
            user.IsActive,
            Is.True);

        repository.Verify(
            x => x.UpdateUser(user),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void ActivateUser_ShouldThrowException_WhenUserNotFound()
    {
        repository
            .Setup(x =>
                x.GetUserById(
                    It.IsAny<Guid>()))
            .Returns(
                (User?)null);

        Assert.Throws<Exception>(
            () =>
                service.ActivateUser(
                    Guid.NewGuid()));
    }

    [Test]
    public void ResetPassword_ShouldResetPassword_WhenUserExists()
    {
        var user =
            new User
            {
                PasswordHash = "old"
            };

        repository
            .Setup(x =>
                x.GetUserById(
                    It.IsAny<Guid>()))
            .Returns(user);

        var result =
            service.ResetPassword(
                Guid.NewGuid());

        Assert.That(
            user.PasswordHash,
            Is.Not.EqualTo("old"));

        Assert.That(
            result.TemporaryPassword,
            Is.Not.Empty);

        repository.Verify(
            x => x.UpdateUser(user),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void ResetPassword_ShouldReturnTemporaryPassword()
    {
        repository
            .Setup(x =>
                x.GetUserById(
                    It.IsAny<Guid>()))
            .Returns(
                new User());

        var result =
            service.ResetPassword(
                Guid.NewGuid());

        Assert.That(
            result.TemporaryPassword,
            Does.StartWith("HRMS@"));
    }

    [Test]
    public void ResetPassword_ShouldThrowException_WhenUserNotFound()
    {
        repository
            .Setup(x =>
                x.GetUserById(
                    It.IsAny<Guid>()))
            .Returns(
                (User?)null);

        Assert.Throws<Exception>(
            () =>
                service.ResetPassword(
                    Guid.NewGuid()));
    }

    [Test]
    public void ChangeRole_ShouldUpdateRole_WhenValidRoleExists()
    {
        var role =
            new Role
            {
                Name = "Manager"
            };

        var user =
            new User
            {
                Roles =
                [
                    new Role
                    {
                        Name = "Employee"
                    }
                ]
            };

        repository
            .Setup(x =>
                x.GetUserById(
                    It.IsAny<Guid>()))
            .Returns(user);

        repository
            .Setup(x =>
                x.GetRoleByName(
                    "Manager"))
            .Returns(role);

        service.ChangeRole(
            Guid.NewGuid(),
            new ChangeRoleDto
            {
                Role = "Manager"
            });

        Assert.That(
            user.Roles.Count,
            Is.EqualTo(1));

        Assert.That(
            user.Roles.First().Name,
            Is.EqualTo("Manager"));

        repository.Verify(
            x => x.UpdateUser(user),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void ChangeRole_ShouldThrowException_WhenUserNotFound()
    {
        repository
            .Setup(x =>
                x.GetUserById(
                    It.IsAny<Guid>()))
            .Returns(
                (User?)null);

        Assert.Throws<Exception>(
            () =>
                service.ChangeRole(
                    Guid.NewGuid(),
                    new ChangeRoleDto
                    {
                        Role = "Admin"
                    }));
    }

    [Test]
    public void ChangeRole_ShouldThrowException_WhenRoleNotFound()
    {
        repository
            .Setup(x =>
                x.GetUserById(
                    It.IsAny<Guid>()))
            .Returns(
                new User());

        repository
            .Setup(x =>
                x.GetRoleByName(
                    It.IsAny<string>()))
            .Returns(
                (Role?)null);

        Assert.Throws<Exception>(
            () =>
                service.ChangeRole(
                    Guid.NewGuid(),
                    new ChangeRoleDto
                    {
                        Role = "Admin"
                    }));
    }
}