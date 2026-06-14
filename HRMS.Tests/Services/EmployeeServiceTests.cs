using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.Employee;
using HRMS.API.Exceptions;
using HRMS.API.Validators;

namespace HRMS.Tests.Services;

[TestFixture]
public class EmployeeServiceTests
{
private Mock<IEmployeeRepository> repository;

private Mock<ILeaveBalanceService>
    leaveBalanceService;

private Mock<INotificationService>
    notificationService;

private EmployeeValidator validator;

private EmployeeService service;

[SetUp]
public void Setup()
{
    repository =
        new Mock<IEmployeeRepository>();

    leaveBalanceService =
        new Mock<ILeaveBalanceService>();

    notificationService =
        new Mock<INotificationService>();

    validator =
        new EmployeeValidator(
            repository.Object);

    service =
        new EmployeeService(
            repository.Object,
            leaveBalanceService.Object,
            notificationService.Object,
            validator);
}

[Test]
public async Task GetAllEmployeesAsync_ShouldReturnMappedDtos()
{
    var employee =
        new Employee
        {
            Id = Guid.NewGuid(),
            EmployeeCode = "EMP001",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Department =
                new Department
                {
                    Name = "IT"
                }
        };

    repository
        .Setup(x =>
            x.GetAllEmployeesAsync(
                null,
                1,
                10))
        .ReturnsAsync(
            (
                new List<Employee>
                {
                    employee
                },
                1));

    var result =
        await service
            .GetAllEmployeesAsync(
                null,
                1,
                10);

    Assert.That(
        result.Data.Count,
        Is.EqualTo(1));

    Assert.That(
        result.Data[0].FirstName,
        Is.EqualTo("John"));

    Assert.That(
        result.Data[0].Department,
        Is.EqualTo("IT"));

    Assert.That(
        result.TotalRecords,
        Is.EqualTo(1));
}

[Test]
public async Task GetEmployeeByIdAsync_ShouldReturnEmployee()
{
    var employeeId =
        Guid.NewGuid();

    repository
        .Setup(x =>
            x.GetEmployeeByIdAsync(
                employeeId))
        .ReturnsAsync(
            new Employee
            {
                Id = employeeId,
                EmployeeCode = "EMP001",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                Department =
                    new Department
                    {
                        Name = "IT"
                    }
            });

    var result =
        await service
            .GetEmployeeByIdAsync(
                employeeId);

    Assert.That(
        result,
        Is.Not.Null);

    Assert.That(
        result!.FirstName,
        Is.EqualTo("John"));
}

[Test]
public void GetEmployeeByIdAsync_ShouldThrowNotFoundException()
{
    repository
        .Setup(x =>
            x.GetEmployeeByIdAsync(
                It.IsAny<Guid>()))
        .ReturnsAsync(
            (Employee?)null);

    Assert.ThrowsAsync<
        NotFoundException>(
        async () =>
            await service
                .GetEmployeeByIdAsync(
                    Guid.NewGuid()));
}

[Test]
public async Task AddEmployeeAsync_ShouldCreateEmployee_WhenValid()
{
    var departmentId =
        Guid.NewGuid();

    var role =
        new Role
        {
            Id = Guid.NewGuid(),
            Name = "Employee"
        };

    repository
        .Setup(x =>
            x.EmployeeExistsAsync(
                It.IsAny<string>()))
        .ReturnsAsync(false);

    repository
        .Setup(x =>
            x.DepartmentExistsAsync(
                departmentId))
        .ReturnsAsync(true);

    repository
        .Setup(x =>
            x.GetRoleByNameAsync(
                "Employee"))
        .ReturnsAsync(role);

    var dto =
        new AddEmployeeDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Phone = "9999999999",
            Designation = "Developer",
            DepartmentId = departmentId,
            Salary = 50000,
            Role = "Employee"
        };

    var result =
        await service
            .AddEmployeeAsync(dto);

    Assert.That(
        result.Message,
        Is.EqualTo(
            "Employee Created Successfully"));

    repository.Verify(
        x => x.AddUserAsync(
            It.IsAny<User>()),
        Times.Once);

    repository.Verify(
        x => x.AddEmployeeAsync(
            It.IsAny<Employee>()),
        Times.Once);

    repository.Verify(
        x => x.SaveChangesAsync(),
        Times.Once);

    leaveBalanceService.Verify(
        x =>
            x.AllocateDefaultBalancesAsync(
                It.IsAny<Guid>()),
        Times.Once);

    notificationService.Verify(
        x =>
            x.CreateNotification(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
        Times.Once);
}

[Test]
public void AddEmployeeAsync_ShouldThrowBusinessException_WhenEmployeeExists()
{
    repository
        .Setup(x =>
            x.EmployeeExistsAsync(
                It.IsAny<string>()))
        .ReturnsAsync(true);

    var dto =
        new AddEmployeeDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            DepartmentId = Guid.NewGuid(),
            Salary = 50000,
            Role = "Employee"
        };

    Assert.ThrowsAsync<
        BusinessException>(
        async () =>
            await service
                .AddEmployeeAsync(dto));
}

[Test]
public void AddEmployeeAsync_ShouldThrowBusinessException_WhenDepartmentNotFound()
{
    repository
        .Setup(x =>
            x.EmployeeExistsAsync(
                It.IsAny<string>()))
        .ReturnsAsync(false);

    repository
        .Setup(x =>
            x.DepartmentExistsAsync(
                It.IsAny<Guid>()))
        .ReturnsAsync(false);

    var dto =
        new AddEmployeeDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            DepartmentId = Guid.NewGuid(),
            Salary = 50000,
            Role = "Employee"
        };

    Assert.ThrowsAsync<
        BusinessException>(
        async () =>
            await service
                .AddEmployeeAsync(dto));
}

[Test]
public void AddEmployeeAsync_ShouldThrowBusinessException_WhenSalaryInvalid()
{
    repository
        .Setup(x =>
            x.EmployeeExistsAsync(
                It.IsAny<string>()))
        .ReturnsAsync(false);

    repository
        .Setup(x =>
            x.DepartmentExistsAsync(
                It.IsAny<Guid>()))
        .ReturnsAsync(true);

    repository
        .Setup(x =>
            x.GetRoleByNameAsync(
                It.IsAny<string>()))
        .ReturnsAsync(
            new Role());

    var dto =
        new AddEmployeeDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            DepartmentId = Guid.NewGuid(),
            Salary = 0,
            Role = "Employee"
        };

    Assert.ThrowsAsync<
        BusinessException>(
        async () =>
            await service
                .AddEmployeeAsync(dto));
}

[Test]
public void AddEmployeeAsync_ShouldThrowBusinessException_WhenRoleInvalid()
{
    repository
        .Setup(x =>
            x.EmployeeExistsAsync(
                It.IsAny<string>()))
        .ReturnsAsync(false);

    repository
        .Setup(x =>
            x.DepartmentExistsAsync(
                It.IsAny<Guid>()))
        .ReturnsAsync(true);

    repository
        .Setup(x =>
            x.GetRoleByNameAsync(
                It.IsAny<string>()))
        .ReturnsAsync(
            (Role?)null);

    var dto =
        new AddEmployeeDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            DepartmentId = Guid.NewGuid(),
            Salary = 50000,
            Role = "Employee"
        };

    Assert.ThrowsAsync<
        BusinessException>(
        async () =>
            await service
                .AddEmployeeAsync(dto));
}


[Test]
public async Task UpdateEmployeeAsync_ShouldUpdateEmployee_WhenValid()
{
var employee =
new Employee
{
Id = Guid.NewGuid(),
FirstName = "Old",
LastName = "Name",
Salary = 10000
};


repository
    .Setup(x =>
        x.GetEmployeeByIdAsync(
            employee.Id))
    .ReturnsAsync(employee);

var dto =
    new UpdateEmployeeDto
    {
        FirstName = "John",
        LastName = "Doe",
        Phone = "9999999999",
        Designation = "Developer",
        Salary = 50000
    };

await service
    .UpdateEmployeeAsync(
        employee.Id,
        dto);

Assert.That(
    employee.FirstName,
    Is.EqualTo("John"));

Assert.That(
    employee.LastName,
    Is.EqualTo("Doe"));

Assert.That(
    employee.Salary,
    Is.EqualTo(50000));

repository.Verify(
    x => x.UpdateEmployee(
        employee),
    Times.Once);

repository.Verify(
    x => x.SaveChangesAsync(),
    Times.Once);


}

[Test]
public void UpdateEmployeeAsync_ShouldThrowNotFoundException()
{
repository
.Setup(x =>
x.GetEmployeeByIdAsync(
It.IsAny<Guid>()))
.ReturnsAsync(
(Employee?)null);


var dto =
    new UpdateEmployeeDto
    {
        FirstName = "John",
        LastName = "Doe",
        Salary = 50000
    };

Assert.ThrowsAsync<
    NotFoundException>(
    async () =>
        await service
            .UpdateEmployeeAsync(
                Guid.NewGuid(),
                dto));


}

[Test]
public void UpdateEmployeeAsync_ShouldThrowBusinessException_WhenSalaryInvalid()
{
var dto =
new UpdateEmployeeDto
{
FirstName = "John",
LastName = "Doe",
Salary = 0
};


Assert.ThrowsAsync<
    BusinessException>(
    async () =>
        await service
            .UpdateEmployeeAsync(
                Guid.NewGuid(),
                dto));


}

[Test]
public async Task DeleteEmployeeAsync_ShouldDeleteEmployee_WhenValid()
{
var employee =
new Employee
{
Id = Guid.NewGuid(),
FirstName = "John"
};


repository
    .Setup(x =>
        x.GetEmployeeByIdAsync(
            employee.Id))
    .ReturnsAsync(employee);

await service
    .DeleteEmployeeAsync(
        employee.Id);

repository.Verify(
    x =>
        x.SoftDeleteEmployee(
            employee,
            Guid.Empty),
    Times.Once);

repository.Verify(
    x => x.SaveChangesAsync(),
    Times.Once);


}

[Test]
public void DeleteEmployeeAsync_ShouldThrowNotFoundException()
{
repository
.Setup(x =>
x.GetEmployeeByIdAsync(
It.IsAny<Guid>()))
.ReturnsAsync(
(Employee?)null);


Assert.ThrowsAsync<
    NotFoundException>(
    async () =>
        await service
            .DeleteEmployeeAsync(
                Guid.NewGuid()));


}

[Test]
public async Task UpdateEmployeeStatusAsync_ShouldUpdateStatus_WhenValid()
{
var userId =
Guid.NewGuid();


var employee =
    new Employee
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        EmploymentStatus =
            "Active"
    };

repository
    .Setup(x =>
        x.GetEmployeeByIdAsync(
            employee.Id))
    .ReturnsAsync(employee);

var dto =
    new UpdateEmployeeStatusDto
    {
        Status =
            "Terminated"
    };

await service
    .UpdateEmployeeStatusAsync(
        employee.Id,
        dto);

Assert.That(
    employee.EmploymentStatus,
    Is.EqualTo(
        "Terminated"));

repository.Verify(
    x => x.UpdateEmployee(
        employee),
    Times.Once);

repository.Verify(
    x => x.SaveChangesAsync(),
    Times.Once);

notificationService.Verify(
    x =>
        x.CreateNotification(
            userId,
            "Employment Status Updated",
            It.IsAny<string>()),
    Times.Once);


}

[Test]
public void UpdateEmployeeStatusAsync_ShouldThrowBusinessException_WhenStatusInvalid()
{
var dto =
new UpdateEmployeeStatusDto
{
Status =
"ABCXYZ"
};


Assert.ThrowsAsync<
    BusinessException>(
    async () =>
        await service
            .UpdateEmployeeStatusAsync(
                Guid.NewGuid(),
                dto));


}

[Test]
public void UpdateEmployeeStatusAsync_ShouldThrowNotFoundException()
{
repository
.Setup(x =>
x.GetEmployeeByIdAsync(
It.IsAny<Guid>()))
.ReturnsAsync(
(Employee?)null);


var dto =
    new UpdateEmployeeStatusDto
    {
        Status =
            "Active"
    };

Assert.ThrowsAsync<
    NotFoundException>(
    async () =>
        await service
            .UpdateEmployeeStatusAsync(
                Guid.NewGuid(),
                dto));


}

[Test]
public async Task GetFullProfileAsync_ShouldReturnMappedProfile()
{
    var employeeId =
        Guid.NewGuid();

    repository
        .Setup(x =>
            x.GetEmployeeFullProfileAsync(
                employeeId))
        .ReturnsAsync(
            new Employee
            {
                Id = employeeId,

                EmployeeCode =
                    "EMP001",

                FirstName =
                    "John",

                LastName =
                    "Doe",

                Email =
                    "john@test.com",

                Department =
                    new Department
                    {
                        Name = "IT"
                    },

                EmployeeEducations =
                [
                    new EmployeeEducation
                    {
                        Id = Guid.NewGuid(),
                        Degree = "B.Tech"
                    }
                ],

                EmployeeExperiences =
                [
                    new EmployeeExperience
                    {
                        Id = Guid.NewGuid(),
                        CompanyName =
                            "ABC"
                    }
                ],

                EmployeeEmergencyContacts =
                [
                    new EmployeeEmergencyContact
                    {
                        Id = Guid.NewGuid(),
                        ContactName =
                            "Father"
                    }
                ],

                EmployeeAddresses =
                [
                    new EmployeeAddress
                    {
                        Id = Guid.NewGuid(),
                        City = "Hyderabad"
                    }
                ],

                EmployeeDocuments =
                [
                    new EmployeeDocument
                    {
                        Id = Guid.NewGuid(),
                        DocumentName =
                            "Aadhar"
                    }
                ]
            });

    var result =
        await service
            .GetFullProfileAsync(
                employeeId);

    Assert.That(
        result,
        Is.Not.Null);

    Assert.That(
        result!.FirstName,
        Is.EqualTo(
            "John"));

    Assert.That(
        result.Department,
        Is.EqualTo(
            "IT"));

    Assert.That(
        result.Educations.Count,
        Is.EqualTo(1));

    Assert.That(
        result.Experiences.Count,
        Is.EqualTo(1));

    Assert.That(
        result.EmergencyContacts.Count,
        Is.EqualTo(1));

    Assert.That(
        result.Addresses.Count,
        Is.EqualTo(1));

    Assert.That(
        result.Documents.Count,
        Is.EqualTo(1));
}

[Test]
public void GetFullProfileAsync_ShouldThrowNotFoundException()
{
    repository
        .Setup(x =>
            x.GetEmployeeFullProfileAsync(
                It.IsAny<Guid>()))
        .ReturnsAsync(
            (Employee?)null);

    Assert.ThrowsAsync<
        NotFoundException>(
        async () =>
            await service
                .GetFullProfileAsync(
                    Guid.NewGuid()));
}



}
