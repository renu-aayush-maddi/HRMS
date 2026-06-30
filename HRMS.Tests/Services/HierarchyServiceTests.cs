using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;

namespace HRMS.Tests.Services;

[TestFixture]
public class HierarchyServiceTests
{
    private Mock<IEmployeeRepository> repository;
    private HierarchyService service;

    [SetUp]
    public void Setup()
    {
        repository = new Mock<IEmployeeRepository>();
        service = new HierarchyService(repository.Object);
    }

    [Test]
    public async Task GetEntireTreeAsync_ShouldReturnActiveTree()
    {
        // Arrange
        var ceo = new Employee
        {
            Id = Guid.NewGuid(),
            EmployeeCode = "EMP001",
            FirstName = "CEO",
            LastName = "Boss",
            Email = "ceo@company.com",
            EmploymentStatus = "Active",
            IsDeleted = false
        };

        var manager = new Employee
        {
            Id = Guid.NewGuid(),
            ManagerId = ceo.Id,
            EmployeeCode = "EMP002",
            FirstName = "Manager",
            LastName = "One",
            Email = "manager@company.com",
            EmploymentStatus = "Active",
            IsDeleted = false
        };

        repository.Setup(x => x.GetActiveEmployeesForHierarchyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Employee> { ceo, manager });

        // Act
        var result = await service.GetEntireTreeAsync();

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo(ceo.Id));
        Assert.That(result[0].Children.Count, Is.EqualTo(1));
        Assert.That(result[0].Children[0].Id, Is.EqualTo(manager.Id));
    }

    [Test]
    public async Task BuildTree_ShouldDetectAndBreakCircularDependencies()
    {
        // Arrange
        var emp1Id = Guid.NewGuid();
        var emp2Id = Guid.NewGuid();

        var emp1 = new Employee
        {
            Id = emp1Id,
            ManagerId = emp2Id,
            EmployeeCode = "EMP001",
            FirstName = "Employee",
            LastName = "One",
            Email = "emp1@company.com",
            EmploymentStatus = "Active",
            IsDeleted = false
        };

        var emp2 = new Employee
        {
            Id = emp2Id,
            ManagerId = emp1Id,
            EmployeeCode = "EMP002",
            FirstName = "Employee",
            LastName = "Two",
            Email = "emp2@company.com",
            EmploymentStatus = "Active",
            IsDeleted = false
        };

        repository.Setup(x => x.GetActiveEmployeesForHierarchyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Employee> { emp1, emp2 });

        // Act
        var result = await service.GetEntireTreeAsync();

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.True(result.Any(n => n.Warning == "Circular manager relationship detected"));
    }

    [Test]
    public async Task BuildTree_ShouldHandleInactiveOrResignedManagers()
    {
        // Arrange
        var manager = new Employee
        {
            Id = Guid.NewGuid(),
            EmployeeCode = "EMP001",
            FirstName = "Manager",
            LastName = "Old",
            Email = "manager@company.com",
            EmploymentStatus = "Resigned",
            IsDeleted = false
        };

        var report = new Employee
        {
            Id = Guid.NewGuid(),
            ManagerId = manager.Id,
            EmployeeCode = "EMP002",
            FirstName = "Report",
            LastName = "Active",
            Email = "report@company.com",
            EmploymentStatus = "Active",
            IsDeleted = false
        };

        repository.Setup(x => x.GetActiveEmployeesForHierarchyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Employee> { manager, report });

        // Act
        var result = await service.GetEntireTreeAsync();

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo(report.Id));
        Assert.That(result[0].Warning, Does.Contain("Resigned"));
    }
}
