using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Exceptions;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.EmployeeSalary;

namespace HRMS.Tests.Services;

[TestFixture]
public class EmployeeSalaryServiceTests
{
private Mock<IEmployeeSalaryRepository>
repository;


private EmployeeSalaryService
    service;

[SetUp]
public void Setup()
{
    repository =
        new Mock<IEmployeeSalaryRepository>();

    service =
        new EmployeeSalaryService(
            repository.Object);
}

[Test]
public void AssignSalary_ShouldAssignSalary_WhenValid()
{
    var employeeId =
        Guid.NewGuid();

    var structureId =
        Guid.NewGuid();

    repository
        .Setup(x =>
            x.GetEmployee(
                employeeId))
        .Returns(
            new Employee
            {
                Id = employeeId
            });

    repository
        .Setup(x =>
            x.GetSalaryStructure(
                structureId))
        .Returns(
            new SalaryStructure
            {
                Id = structureId
            });

    repository
        .Setup(x =>
            x.GetActiveSalary(
                employeeId))
        .Returns(
            (EmployeeSalary?)null);

    var dto =
        new AssignEmployeeSalaryDto
        {
            EmployeeId =
                employeeId,

            SalaryStructureId =
                structureId,

            AnnualCtc =
                1200000,

            EffectiveFrom =
                new DateOnly(
                    2025,
                    1,
                    1)
        };

    service.AssignSalary(
        dto);

    repository.Verify(
        x => x.Add(
            It.IsAny<EmployeeSalary>()),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void AssignSalary_ShouldThrowNotFoundException_WhenEmployeeNotFound()
{
    repository
        .Setup(x =>
            x.GetEmployee(
                It.IsAny<Guid>()))
        .Returns(
            (Employee?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.AssignSalary(
                new AssignEmployeeSalaryDto
                {
                    EmployeeId =
                        Guid.NewGuid()
                }));
}

[Test]
public void AssignSalary_ShouldThrowNotFoundException_WhenSalaryStructureNotFound()
{
    repository
        .Setup(x =>
            x.GetEmployee(
                It.IsAny<Guid>()))
        .Returns(
            new Employee());

    repository
        .Setup(x =>
            x.GetSalaryStructure(
                It.IsAny<Guid>()))
        .Returns(
            (SalaryStructure?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.AssignSalary(
                new AssignEmployeeSalaryDto
                {
                    EmployeeId =
                        Guid.NewGuid(),

                    SalaryStructureId =
                        Guid.NewGuid()
                }));
}

[Test]
public void AssignSalary_ShouldThrowBusinessException_WhenAnnualCtcIsZeroOrLess()
{
    repository
        .Setup(x =>
            x.GetEmployee(
                It.IsAny<Guid>()))
        .Returns(
            new Employee());

    repository
        .Setup(x =>
            x.GetSalaryStructure(
                It.IsAny<Guid>()))
        .Returns(
            new SalaryStructure());

    Assert.Throws<
        BusinessException>(
        () =>
            service.AssignSalary(
                new AssignEmployeeSalaryDto
                {
                    EmployeeId =
                        Guid.NewGuid(),

                    SalaryStructureId =
                        Guid.NewGuid(),

                    AnnualCtc = 0
                }));
}

[Test]
public void AssignSalary_ShouldDeactivateCurrentSalary_WhenActiveSalaryExists()
{
    var activeSalary =
        new EmployeeSalary
        {
            Id = Guid.NewGuid(),
            IsActive = true
        };

    repository
        .Setup(x =>
            x.GetEmployee(
                It.IsAny<Guid>()))
        .Returns(
            new Employee());

    repository
        .Setup(x =>
            x.GetSalaryStructure(
                It.IsAny<Guid>()))
        .Returns(
            new SalaryStructure());

    repository
        .Setup(x =>
            x.GetActiveSalary(
                It.IsAny<Guid>()))
        .Returns(
            activeSalary);

    service.AssignSalary(
        new AssignEmployeeSalaryDto
        {
            EmployeeId =
                Guid.NewGuid(),

            SalaryStructureId =
                Guid.NewGuid(),

            AnnualCtc =
                1000000
        });

    Assert.That(
        activeSalary.IsActive,
        Is.False);

    repository.Verify(
        x => x.Update(
            activeSalary),
        Times.Once);
}

[Test]
public void GetActiveSalary_ShouldReturnSalary_WhenFound()
{
    var salary =
        new EmployeeSalary
        {
            Id = Guid.NewGuid(),

            AnnualCtc = 1000000,

            EffectiveFrom =
                new DateOnly(
                    2025,
                    1,
                    1),

            IsActive = true,

            Employee =
                new Employee
                {
                    FirstName = "John",
                    LastName = "Doe"
                },

            SalaryStructure =
                new SalaryStructure
                {
                    Name = "Default"
                }
        };

    repository
        .Setup(x =>
            x.GetActiveSalary(
                It.IsAny<Guid>()))
        .Returns(
            salary);

    var result =
        service.GetActiveSalary(
            Guid.NewGuid());

    Assert.That(
        result.EmployeeName,
        Is.EqualTo(
            "John Doe"));

    Assert.That(
        result.SalaryStructureName,
        Is.EqualTo(
            "Default"));
}

[Test]
public void GetActiveSalary_ShouldThrowNotFoundException_WhenSalaryNotFound()
{
    repository
        .Setup(x =>
            x.GetActiveSalary(
                It.IsAny<Guid>()))
        .Returns(
            (EmployeeSalary?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.GetActiveSalary(
                Guid.NewGuid()));
}

[Test]
public void GetAll_ShouldReturnMappedDtos()
{
    repository
        .Setup(x =>
            x.GetAll())
        .Returns(
            new List<EmployeeSalary>
            {
                new EmployeeSalary
                {
                    Id = Guid.NewGuid(),

                    AnnualCtc = 1000000,

                    Employee =
                        new Employee
                        {
                            FirstName = "John",
                            LastName = "Doe"
                        },

                    SalaryStructure =
                        new SalaryStructure
                        {
                            Name = "Default"
                        }
                }
            });

    var result =
        service.GetAll();

    Assert.That(
        result.Count,
        Is.EqualTo(1));

    Assert.That(
        result[0].EmployeeName,
        Is.EqualTo("John Doe"));
}

[Test]
public void GetSalaryHistory_ShouldReturnMappedDtos()
{
    var employeeId =
        Guid.NewGuid();

    repository
        .Setup(x =>
            x.GetEmployee(
                employeeId))
        .Returns(
            new Employee
            {
                Id = employeeId
            });

    repository
        .Setup(x =>
            x.GetSalaryHistory(
                employeeId))
        .Returns(
            new List<EmployeeSalary>
            {
                new EmployeeSalary
                {
                    Id = Guid.NewGuid(),

                    AnnualCtc = 1000000,

                    Employee =
                        new Employee
                        {
                            FirstName = "John",
                            LastName = "Doe"
                        },

                    SalaryStructure =
                        new SalaryStructure
                        {
                            Name = "Default"
                        }
                }
            });

    var result =
        service.GetSalaryHistory(
            employeeId);

    Assert.That(
        result.Count,
        Is.EqualTo(1));

    Assert.That(
        result[0].EmployeeName,
        Is.EqualTo("John Doe"));
}

[Test]
public void GetSalaryHistory_ShouldThrowNotFoundException_WhenEmployeeNotFound()
{
    repository
        .Setup(x =>
            x.GetEmployee(
                It.IsAny<Guid>()))
        .Returns(
            (Employee?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.GetSalaryHistory(
                Guid.NewGuid()));
}


}
