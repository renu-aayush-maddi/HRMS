using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.Department;
using HRMS.API.Exceptions;

namespace HRMS.Tests.Services;

[TestFixture]
public class DepartmentServiceTests
{
    [Test]
    public void GetAllDepartments_ShouldReturnMappedDtos()
    {
        var repository =
            new Mock<IDepartmentRepository>();

        repository
            .Setup(x =>
                x.GetAllDepartments())
            .Returns(
                new List<Department>
                {
                    new Department
                    {
                        Id = Guid.NewGuid(),
                        Name = "IT"
                    }
                });

        var service =
            new DepartmentService(
                repository.Object);

        var result =
            service.GetAllDepartments();

        Assert.That(
            result.Count,
            Is.EqualTo(1));

        Assert.That(
            result[0].Name,
            Is.EqualTo("IT"));
    }

    [Test]
    public void AddDepartment_ShouldAddDepartment_WhenValid()
    {
        var repository =
            new Mock<IDepartmentRepository>();

        repository
            .Setup(x =>
                x.DepartmentExists(
                    It.IsAny<string>()))
            .Returns(false);

        var service =
            new DepartmentService(
                repository.Object);

        service.AddDepartment(
            new AddDepartmentDto
            {
                Name = "IT"
            });

        repository.Verify(
            x => x.AddDepartment(
                It.IsAny<Department>()),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void AddDepartment_ShouldThrowBusinessException_WhenDepartmentExists()
    {
        var repository =
            new Mock<IDepartmentRepository>();

        repository
            .Setup(x =>
                x.DepartmentExists(
                    It.IsAny<string>()))
            .Returns(true);

        var service =
            new DepartmentService(
                repository.Object);

        Assert.Throws<BusinessException>(
            () =>
            service.AddDepartment(
                new AddDepartmentDto
                {
                    Name = "IT"
                }));
    }

    [Test]
    public void UpdateDepartment_ShouldUpdateDepartment_WhenValid()
    {
        var repository =
            new Mock<IDepartmentRepository>();

        var department =
            new Department
            {
                Id = Guid.NewGuid(),
                Name = "HR"
            };

        repository
            .Setup(x =>
                x.GetDepartmentById(
                    department.Id))
            .Returns(department);

        repository
            .Setup(x =>
                x.DepartmentExists(
                    It.IsAny<string>()))
            .Returns(false);

        var service =
            new DepartmentService(
                repository.Object);

        service.UpdateDepartment(
            department.Id,
            new UpdateDepartmentDto
            {
                Name = "IT"
            });

        repository.Verify(
            x => x.UpdateDepartment(
                department),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void UpdateDepartment_ShouldThrowNotFoundException_WhenDepartmentNotFound()
    {
        var repository =
            new Mock<IDepartmentRepository>();

        repository
            .Setup(x =>
                x.GetDepartmentById(
                    It.IsAny<Guid>()))
            .Returns((Department?)null);

        var service =
            new DepartmentService(
                repository.Object);

        Assert.Throws<NotFoundException>(
            () =>
            service.UpdateDepartment(
                Guid.NewGuid(),
                new UpdateDepartmentDto
                {
                    Name = "IT"
                }));
    }

    [Test]
    public void UpdateDepartment_ShouldThrowBusinessException_WhenDuplicateNameExists()
    {
        var repository =
            new Mock<IDepartmentRepository>();

        var department =
            new Department
            {
                Id = Guid.NewGuid(),
                Name = "HR"
            };

        repository
            .Setup(x =>
                x.GetDepartmentById(
                    department.Id))
            .Returns(department);

        repository
            .Setup(x =>
                x.DepartmentExists(
                    "IT"))
            .Returns(true);

        var service =
            new DepartmentService(
                repository.Object);

        Assert.Throws<BusinessException>(
            () =>
            service.UpdateDepartment(
                department.Id,
                new UpdateDepartmentDto
                {
                    Name = "IT"
                }));
    }

    [Test]
    public void DeleteDepartment_ShouldDeleteDepartment_WhenValid()
    {
        var repository =
            new Mock<IDepartmentRepository>();

        var department =
            new Department
            {
                Id = Guid.NewGuid(),
                Name = "IT"
            };

        repository
            .Setup(x =>
                x.GetDepartmentById(
                    department.Id))
            .Returns(department);

        repository
            .Setup(x =>
                x.HasEmployees(
                    department.Id))
            .Returns(false);

        var service =
            new DepartmentService(
                repository.Object);

        service.DeleteDepartment(
            department.Id);

        repository.Verify(
            x => x.DeleteDepartment(
                department),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void DeleteDepartment_ShouldThrowNotFoundException_WhenDepartmentNotFound()
    {
        var repository =
            new Mock<IDepartmentRepository>();

        repository
            .Setup(x =>
                x.GetDepartmentById(
                    It.IsAny<Guid>()))
            .Returns((Department?)null);

        var service =
            new DepartmentService(
                repository.Object);

        Assert.Throws<NotFoundException>(
            () =>
            service.DeleteDepartment(
                Guid.NewGuid()));
    }

    [Test]
    public void DeleteDepartment_ShouldThrowBusinessException_WhenDepartmentHasEmployees()
    {
        var repository =
            new Mock<IDepartmentRepository>();

        var department =
            new Department
            {
                Id = Guid.NewGuid(),
                Name = "IT"
            };

        repository
            .Setup(x =>
                x.GetDepartmentById(
                    department.Id))
            .Returns(department);

        repository
            .Setup(x =>
                x.HasEmployees(
                    department.Id))
            .Returns(true);

        var service =
            new DepartmentService(
                repository.Object);

        Assert.Throws<BusinessException>(
            () =>
            service.DeleteDepartment(
                department.Id));
    }
}