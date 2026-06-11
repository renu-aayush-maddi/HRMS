using NUnit.Framework;
using Moq;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Helpers;
using HRMS.API.Models.DTOs.Auth;
using HRMS.API.Models.Entities;
using HRMS.API.Exceptions;
using Microsoft.Extensions.Configuration;

namespace HRMS.Tests.Services;

[TestFixture]
public class AuthServiceTests
{
    private JwtHelper CreateJwtHelper()
    {
        var settings =
            new Dictionary<string, string?>
            {
                {
                    "Jwt:Key",
                    "ThisIsASuperSecretKeyForTesting12345"
                },
                {
                    "Jwt:Issuer",
                    "TestIssuer"
                },
                {
                    "Jwt:Audience",
                    "TestAudience"
                }
            };

        IConfiguration configuration =
            new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        return new JwtHelper(configuration);
    }

    [Test]
    public async Task Register_ShouldCreateUser_WhenValid()
    {
        var repository =
            new Mock<IAuthRepository>();

        repository
            .Setup(x =>
                x.GetUserByEmail(
                    It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        repository
            .Setup(x =>
                x.GetRoleByName(
                    It.IsAny<string>()))
            .ReturnsAsync(
                new Role
                {
                    Name = "Employee"
                });

        var service =
            new AuthService(
                repository.Object,
                CreateJwtHelper());

        var dto =
            new RegisterDto
            {
                Email = "test@test.com",
                Password = "Password123",
                FirstName = "John",
                LastName = "Doe",
                Role = "Employee",
                DepartmentId = Guid.NewGuid(),
                Designation = "Developer",
                Salary = 50000
            };

        var result =
            await service.Register(dto);

        Assert.That(
            result,
            Is.EqualTo(
                "User Registered Successfully"));

        repository.Verify(
            x => x.AddUser(
                It.IsAny<User>()),
            Times.Once);

        repository.Verify(
            x => x.AddEmployee(
                It.IsAny<Employee>()),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public async Task Register_ShouldThrowBusinessException_WhenUserExists()
    {
        var repository = new Mock<IAuthRepository>();

        repository
            .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync(new User());

        var service =
            new AuthService(
                repository.Object,
                CreateJwtHelper());

        Assert.That(
            async () => await service.Register(new RegisterDto()),
            Throws.TypeOf<BusinessException>());
    }

    [Test]
    public async Task Register_ShouldThrowBusinessException_WhenRoleInvalid()
    {
        var repository = new Mock<IAuthRepository>();

        repository
            .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        repository
            .Setup(x => x.GetRoleByName(It.IsAny<string>()))
            .ReturnsAsync((Role?)null);

        var service =
            new AuthService(
                repository.Object,
                CreateJwtHelper());

        Assert.That(
            async () => await service.Register(
                new RegisterDto
                {
                    Email = "test@test.com",
                    Password = "Password123",
                    FirstName = "John",
                    LastName = "Doe",
                    Role = "Employee",
                    DepartmentId = Guid.NewGuid(),
                    Designation = "Developer",
                    Salary = 50000
                }),
            Throws.TypeOf<BusinessException>());
    }

    [Test]
    public async Task Login_ShouldReturnToken_WhenValid()
    {
        var repository =
            new Mock<IAuthRepository>();

        var password =
            BCrypt.Net.BCrypt
            .HashPassword(
                "Password123");

        var user =
            new User
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com",
                PasswordHash = password,
                IsActive = true
            };

        repository
            .Setup(x =>
                x.GetUserByEmail(
                    It.IsAny<string>()))
            .ReturnsAsync(user);

        repository
            .Setup(x =>
                x.GetUserRole(
                    user.Id))
            .ReturnsAsync(
                "Employee");

        var service =
            new AuthService(
                repository.Object,
                CreateJwtHelper());

        var result =
            await service.Login(
                new LoginDto
                {
                    Email = "test@test.com",
                    Password = "Password123"
                });

        Assert.That(
            result.Token,
            Is.Not.Null);

        Assert.That(
            result.Role,
            Is.EqualTo(
                "Employee"));
    }

    [Test]
    public async Task Login_ShouldThrowBusinessException_WhenUserNotFound()
    {
        var repository = new Mock<IAuthRepository>();

        repository
            .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var service =
            new AuthService(
                repository.Object,
                CreateJwtHelper());

        Assert.That(
            async () => await service.Login(new LoginDto()),
            Throws.TypeOf<BusinessException>());
    }

    [Test]
    public async Task Login_ShouldThrowBusinessException_WhenUserDisabled()
    {
        var repository = new Mock<IAuthRepository>();

        repository
            .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync(
                new User
                {
                    IsActive = false
                });

        var service =
            new AuthService(
                repository.Object,
                CreateJwtHelper());

        Assert.That(
            async () => await service.Login(new LoginDto()),
            Throws.TypeOf<BusinessException>());
    }

    [Test]
    public async Task Login_ShouldThrowBusinessException_WhenPasswordInvalid()
    {
        var repository = new Mock<IAuthRepository>();

        var user =
            new User
            {
                Email = "test@test.com",
                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        "CorrectPassword"),
                IsActive = true
            };

        repository
            .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync(user);

        var service =
            new AuthService(
                repository.Object,
                CreateJwtHelper());

        Assert.That(
            async () => await service.Login(
                new LoginDto
                {
                    Email = "test@test.com",
                    Password = "WrongPassword"
                }),
            Throws.TypeOf<BusinessException>());
    }
}