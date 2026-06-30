#if false
using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Resignation;
using HRMS.API.Models.Entities;

namespace HRMS.Tests.Services;

[TestFixture]
public class ResignationServiceTests
{
    private Mock<IResignationRepository>
        repository;

    private ResignationService
        service;

    [SetUp]
    public void Setup()
    {
        repository =
            new Mock<IResignationRepository>();

        service =
            new ResignationService(
                repository.Object);
    }

    [Test]
    public void SubmitResignation_ShouldAddResignation_WhenEmployeeExists()
    {
        var employeeId =
            Guid.NewGuid();

        var dto =
            new SubmitResignationDto
            {
                EmployeeId =
                    employeeId,

                LastWorkingDate =
                    DateOnly.FromDateTime(
                        DateTime.Today.AddDays(30)),

                Reason =
                    "Personal"
            };

        repository
            .Setup(x =>
                x.GetEmployee(
                    employeeId))
            .Returns(
                new Employee
                {
                    Id = employeeId
                });

        service.SubmitResignation(
            dto);

        repository.Verify(
            x => x.Add(
                It.IsAny<EmployeeResignation>()),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void SubmitResignation_ShouldThrowException_WhenEmployeeNotFound()
    {
        var dto =
            new SubmitResignationDto
            {
                EmployeeId =
                    Guid.NewGuid()
            };

        repository
            .Setup(x =>
                x.GetEmployee(
                    dto.EmployeeId))
            .Returns(
                (Employee?)null);

        Assert.Throws<Exception>(
            () =>
                service.SubmitResignation(
                    dto));
    }

    [Test]
    public void GetAll_ShouldReturnMappedDtos()
    {
        repository
            .Setup(x =>
                x.GetAll())
            .Returns(
            [
                new EmployeeResignation
                {
                    Id = Guid.NewGuid(),

                    Status = "Pending",

                    Employee =
                        new Employee
                        {
                            FirstName = "John",
                            LastName = "Doe"
                        }
                }
            ]);

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
    public void GetAll_ShouldReturnEmptyList_WhenNoResignationsExist()
    {
        repository
            .Setup(x =>
                x.GetAll())
            .Returns([]);

        var result =
            service.GetAll();

        Assert.That(
            result,
            Is.Empty);
    }

    [Test]
    public void GetEmployeeResignations_ShouldReturnMappedDtos()
    {
        var employeeId =
            Guid.NewGuid();

        repository
            .Setup(x =>
                x.GetEmployeeResignations(
                    employeeId))
            .Returns(
            [
                new EmployeeResignation
                {
                    Employee =
                        new Employee
                        {
                            FirstName = "John",
                            LastName = "Doe"
                        }
                }
            ]);

        var result =
            service.GetEmployeeResignations(
                employeeId);

        Assert.That(
            result.Count,
            Is.EqualTo(1));

        Assert.That(
            result[0].EmployeeName,
            Is.EqualTo("John Doe"));
    }

    [Test]
    public void GetEmployeeResignations_ShouldReturnEmptyList_WhenNoResignationsExist()
    {
        repository
            .Setup(x =>
                x.GetEmployeeResignations(
                    It.IsAny<Guid>()))
            .Returns([]);

        var result =
            service.GetEmployeeResignations(
                Guid.NewGuid());

        Assert.That(
            result,
            Is.Empty);
    }

    [Test]
    public void Approve_ShouldApproveResignation_WhenValid()
    {
        var resignation =
            new EmployeeResignation
            {
                Employee =
                    new Employee()
            };

        repository
            .Setup(x =>
                x.GetResignation(
                    It.IsAny<Guid>()))
            .Returns(
                resignation);

        service.Approve(
            Guid.NewGuid(),
            new ResignationActionDto
            {
                HrComments =
                    "Approved"
            });

        Assert.That(
            resignation.Status,
            Is.EqualTo("Approved"));

        repository.Verify(
            x => x.Update(
                resignation),
            Times.Once);
    }

    [Test]
    public void Approve_ShouldUpdateEmployeeStatusToNoticePeriod()
    {
        var employee =
            new Employee();

        var resignation =
            new EmployeeResignation
            {
                Employee =
                    employee
            };

        repository
            .Setup(x =>
                x.GetResignation(
                    It.IsAny<Guid>()))
            .Returns(
                resignation);

        service.Approve(
            Guid.NewGuid(),
            new ResignationActionDto());

        Assert.That(
            employee.EmploymentStatus,
            Is.EqualTo("NoticePeriod"));
    }

    [Test]
    public void Approve_ShouldThrowException_WhenResignationNotFound()
    {
        repository
            .Setup(x =>
                x.GetResignation(
                    It.IsAny<Guid>()))
            .Returns(
                (EmployeeResignation?)null);

        Assert.Throws<Exception>(
            () =>
                service.Approve(
                    Guid.NewGuid(),
                    new ResignationActionDto()));
    }

    [Test]
    public void Reject_ShouldRejectResignation_WhenValid()
    {
        var resignation =
            new EmployeeResignation();

        repository
            .Setup(x =>
                x.GetResignation(
                    It.IsAny<Guid>()))
            .Returns(
                resignation);

        service.Reject(
            Guid.NewGuid(),
            new ResignationActionDto
            {
                HrComments =
                    "Rejected"
            });

        Assert.That(
            resignation.Status,
            Is.EqualTo("Rejected"));

        repository.Verify(
            x => x.Update(
                resignation),
            Times.Once);
    }

    [Test]
    public void Reject_ShouldThrowException_WhenResignationNotFound()
    {
        repository
            .Setup(x =>
                x.GetResignation(
                    It.IsAny<Guid>()))
            .Returns(
                (EmployeeResignation?)null);

        Assert.Throws<Exception>(
            () =>
                service.Reject(
                    Guid.NewGuid(),
                    new ResignationActionDto()));
    }

    [Test]
    public void UpdateSettlement_ShouldUpdateSettlementStatus_WhenValid()
    {
        var resignation =
            new EmployeeResignation();

        repository
            .Setup(x =>
                x.GetResignation(
                    It.IsAny<Guid>()))
            .Returns(
                resignation);

        service.UpdateSettlement(
            Guid.NewGuid(),
            new SettlementDto
            {
                SettlementStatus =
                    "Completed"
            });

        Assert.That(
            resignation.FinalSettlementStatus,
            Is.EqualTo("Completed"));
    }

    [Test]
    public void UpdateSettlement_ShouldThrowException_WhenResignationNotFound()
    {
        repository
            .Setup(x =>
                x.GetResignation(
                    It.IsAny<Guid>()))
            .Returns(
                (EmployeeResignation?)null);

        Assert.Throws<Exception>(
            () =>
                service.UpdateSettlement(
                    Guid.NewGuid(),
                    new SettlementDto()));
    }
}
#endif