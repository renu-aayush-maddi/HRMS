using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.LeaveType;
using HRMS.API.Exceptions;

namespace HRMS.Tests.Services;

[TestFixture]
public class LeaveTypeServiceTests
{
    [Test]
    public void GetAll_ShouldReturnMappedDtos()
    {
        var repository =
            new Mock<ILeaveTypeRepository>();

        repository
            .Setup(x => x.GetAll())
            .Returns(
                new List<LeaveType>
                {
                    new LeaveType
                    {
                        Id = Guid.NewGuid(),

                        Name = "Sick Leave",

                        AnnualAllocation = 12,

                        CarryForwardAllowed = true,

                        MaxCarryForward = 5,

                        NegativeBalanceAllowed = false,

                        IsActive = true
                    }
                });

        var service =
            new LeaveTypeService(
                repository.Object);

        var result =
            service.GetAll();

        Assert.That(
            result.Count,
            Is.EqualTo(1));

        Assert.That(
            result[0].Name,
            Is.EqualTo("Sick Leave"));
    }

    [Test]
    public void Add_ShouldAddLeaveType_WhenValid()
    {
        var repository =
            new Mock<ILeaveTypeRepository>();

        repository
            .Setup(x =>
                x.Exists(
                    It.IsAny<string>()))
            .Returns(false);

        var service =
            new LeaveTypeService(
                repository.Object);

        service.Add(
            new AddLeaveTypeDto
            {
                Name = "Sick Leave",

                AnnualAllocation = 12,

                CarryForwardAllowed = true,

                MaxCarryForward = 5,

                NegativeBalanceAllowed = false
            });

        repository.Verify(
            x => x.Add(
                It.IsAny<LeaveType>()),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void Add_ShouldThrowBusinessException_WhenDuplicate()
    {
        var repository =
            new Mock<ILeaveTypeRepository>();

        repository
            .Setup(x =>
                x.Exists(
                    It.IsAny<string>()))
            .Returns(true);

        var service =
            new LeaveTypeService(
                repository.Object);

        Assert.Throws<BusinessException>(
            () =>
            service.Add(
                new AddLeaveTypeDto
                {
                    Name = "Sick Leave"
                }));
    }

    [Test]
    public void Add_ShouldThrowBusinessException_WhenCarryForwardInvalid()
    {
        var repository =
            new Mock<ILeaveTypeRepository>();

        repository
            .Setup(x =>
                x.Exists(
                    It.IsAny<string>()))
            .Returns(false);

        var service =
            new LeaveTypeService(
                repository.Object);

        Assert.Throws<BusinessException>(
            () =>
            service.Add(
                new AddLeaveTypeDto
                {
                    Name = "Sick Leave",

                    AnnualAllocation = 12,

                    CarryForwardAllowed = false,

                    MaxCarryForward = 10
                }));
    }

    [Test]
    public void Update_ShouldUpdateLeaveType_WhenValid()
    {
        var repository =
            new Mock<ILeaveTypeRepository>();

        var leaveType =
            new LeaveType
            {
                Id = Guid.NewGuid(),

                Name = "Old Leave"
            };

        repository
            .Setup(x =>
                x.GetById(
                    leaveType.Id))
            .Returns(leaveType);

        repository
            .Setup(x =>
                x.Exists(
                    It.IsAny<string>()))
            .Returns(false);

        var service =
            new LeaveTypeService(
                repository.Object);

        service.Update(
            leaveType.Id,
            new UpdateLeaveTypeDto
            {
                Name = "Sick Leave",

                AnnualAllocation = 12,

                CarryForwardAllowed = true,

                MaxCarryForward = 5,

                NegativeBalanceAllowed = false,

                IsActive = true
            });

        Assert.That(
            leaveType.Name,
            Is.EqualTo("Sick Leave"));

        repository.Verify(
            x => x.Update(
                leaveType),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void Update_ShouldThrowNotFoundException()
    {
        var repository =
            new Mock<ILeaveTypeRepository>();

        repository
            .Setup(x =>
                x.GetById(
                    It.IsAny<Guid>()))
            .Returns((LeaveType?)null);

        var service =
            new LeaveTypeService(
                repository.Object);

        Assert.Throws<NotFoundException>(
            () =>
            service.Update(
                Guid.NewGuid(),
                new UpdateLeaveTypeDto()));
    }

    [Test]
    public void Update_ShouldThrowBusinessException_WhenDuplicate()
    {
        var repository =
            new Mock<ILeaveTypeRepository>();

        var leaveType =
            new LeaveType
            {
                Id = Guid.NewGuid(),

                Name = "Casual Leave"
            };

        repository
            .Setup(x =>
                x.GetById(
                    leaveType.Id))
            .Returns(leaveType);

        repository
            .Setup(x =>
                x.Exists(
                    "Sick Leave"))
            .Returns(true);

        var service =
            new LeaveTypeService(
                repository.Object);

        Assert.Throws<BusinessException>(
            () =>
            service.Update(
                leaveType.Id,
                new UpdateLeaveTypeDto
                {
                    Name = "Sick Leave"
                }));
    }

    [Test]
    public void Delete_ShouldDeleteLeaveType_WhenValid()
    {
        var repository =
            new Mock<ILeaveTypeRepository>();

        var leaveType =
            new LeaveType
            {
                Id = Guid.NewGuid(),

                Name = "Sick Leave"
            };

        repository
            .Setup(x =>
                x.GetById(
                    leaveType.Id))
            .Returns(leaveType);

        repository
            .Setup(x =>
                x.IsUsed(
                    leaveType.Id))
            .Returns(false);

        var service =
            new LeaveTypeService(
                repository.Object);

        service.Delete(
            leaveType.Id);

        repository.Verify(
            x => x.Delete(
                leaveType),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void Delete_ShouldThrowNotFoundException()
    {
        var repository =
            new Mock<ILeaveTypeRepository>();

        repository
            .Setup(x =>
                x.GetById(
                    It.IsAny<Guid>()))
            .Returns((LeaveType?)null);

        var service =
            new LeaveTypeService(
                repository.Object);

        Assert.Throws<NotFoundException>(
            () =>
            service.Delete(
                Guid.NewGuid()));
    }

    [Test]
    public void Delete_ShouldThrowBusinessException_WhenInUse()
    {
        var repository =
            new Mock<ILeaveTypeRepository>();

        var leaveType =
            new LeaveType
            {
                Id = Guid.NewGuid(),

                Name = "Sick Leave"
            };

        repository
            .Setup(x =>
                x.GetById(
                    leaveType.Id))
            .Returns(leaveType);

        repository
            .Setup(x =>
                x.IsUsed(
                    leaveType.Id))
            .Returns(true);

        var service =
            new LeaveTypeService(
                repository.Object);

        Assert.Throws<BusinessException>(
            () =>
            service.Delete(
                leaveType.Id));
    }
}