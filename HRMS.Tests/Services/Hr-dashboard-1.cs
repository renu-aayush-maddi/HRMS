using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Dashboard;

namespace HRMS.Tests.Services;

[TestFixture]
public class HrDashboardServiceTests
{
    private Mock<IHrDashboardRepository>
        repository;

    private HrDashboardService
        service;

    [SetUp]
    public void Setup()
    {
        repository =
            new Mock<IHrDashboardRepository>();

        service =
            new HrDashboardService(
                repository.Object);
    }

    [Test]
    public void GetStats_ShouldReturnDashboardStats()
    {
        var stats =
            new HrDashboardStatsDto
            {
                TotalEmployees = 100,
                ActiveEmployees = 90
            };

        repository
            .Setup(x =>
                x.GetStats())
            .Returns(stats);

        var result =
            service.GetStats();

        Assert.That(
            result.TotalEmployees,
            Is.EqualTo(100));

        Assert.That(
            result.ActiveEmployees,
            Is.EqualTo(90));
    }

    [Test]
    public void GetDepartmentSummary_ShouldReturnDepartmentSummary()
    {
        var departments =
            new List<DepartmentSummaryDto>
            {
                new()
                {
                    DepartmentName = "IT",
                    EmployeeCount = 20
                }
            };

        repository
            .Setup(x =>
                x.GetDepartmentSummary())
            .Returns(departments);

        var result =
            service.GetDepartmentSummary();

        Assert.That(
            result.Count,
            Is.EqualTo(1));

        Assert.That(
            result[0].DepartmentName,
            Is.EqualTo("IT"));
    }

    [Test]
    public void GetDepartmentSummary_ShouldReturnEmptyList_WhenNoDepartmentsExist()
    {
        repository
            .Setup(x =>
                x.GetDepartmentSummary())
            .Returns([]);

        var result =
            service.GetDepartmentSummary();

        Assert.That(
            result,
            Is.Empty);
    }

    [Test]
    public void GetLeaveSummary_ShouldReturnLeaveSummary()
    {
        var summary =
            new LeaveSummaryDto
            {
                PendingLeaves = 5,
                ApprovedLeaves = 10
            };

        repository
            .Setup(x =>
                x.GetLeaveSummary())
            .Returns(summary);

        var result =
            service.GetLeaveSummary();

        Assert.That(
            result.PendingLeaves,
            Is.EqualTo(5));

        Assert.That(
            result.ApprovedLeaves,
            Is.EqualTo(10));
    }

    [Test]
    public void GetPayrollSummary_ShouldReturnPayrollSummary()
    {
        var summary =
            new PayrollSummaryDto
            {
                TotalPayroll = 100000,
                EmployeesPaid = 50
            };

        repository
            .Setup(x =>
                x.GetPayrollSummary())
            .Returns(summary);

        var result =
            service.GetPayrollSummary();

        Assert.That(
            result.TotalPayroll,
            Is.EqualTo(100000));

        Assert.That(
            result.EmployeesPaid,
            Is.EqualTo(50));
    }
}