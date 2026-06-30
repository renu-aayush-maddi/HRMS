using Moq;
using NUnit.Framework;
using QuestPDF.Infrastructure;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Exceptions;
using HRMS.API.Models.Entities;

namespace HRMS.Tests.Services;

[TestFixture]
public class PayslipServiceTests
{
    private Mock<IPayrollRepository>
        repository;

    private PayslipService
        service;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        QuestPDF.Settings.License =
            LicenseType.Community;
    }

    [SetUp]
    public void Setup()
    {
        repository =
            new Mock<IPayrollRepository>();

        service =
            new PayslipService(
                repository.Object);
    }

    [Test]
    public void GeneratePayslip_ShouldReturnPdf_WhenPayrollExists()
    {
        var payroll =
            CreateValidPayroll();

        repository
            .Setup(x =>
                x.GetPayrollById(
                    payroll.Id))
            .Returns(
                payroll);

        var result =
            service.GeneratePayslip(
                payroll.Id);

        Assert.That(
            result,
            Is.Not.Null);

        Assert.That(
            result.Length,
            Is.GreaterThan(0));
    }

    [Test]
    public void GeneratePayslip_ShouldThrowNotFoundException_WhenPayrollNotFound()
    {
        repository
            .Setup(x =>
                x.GetPayrollById(
                    It.IsAny<Guid>()))
            .Returns(
                (Payroll?)null);

        Assert.Throws<NotFoundException>(
            () =>
                service.GeneratePayslip(
                    Guid.NewGuid()));
    }

    [Test]
    public void GenerateMyPayslip_ShouldReturnPdf_WhenUserOwnsPayroll()
    {
        var userId =
            Guid.NewGuid();

        var payroll =
            CreateValidPayroll();

        payroll.Employee!.UserId =
            userId;

        repository
            .Setup(x =>
                x.GetPayrollById(
                    payroll.Id))
            .Returns(
                payroll);

        var result =
            service.GenerateMyPayslip(
                payroll.Id,
                userId);

        Assert.That(
            result,
            Is.Not.Null);

        Assert.That(
            result.Length,
            Is.GreaterThan(0));
    }

    [Test]
    public void GenerateMyPayslip_ShouldThrowNotFoundException_WhenPayrollNotFound()
    {
        repository
            .Setup(x =>
                x.GetPayrollById(
                    It.IsAny<Guid>()))
            .Returns(
                (Payroll?)null);

        Assert.Throws<NotFoundException>(
            () =>
                service.GenerateMyPayslip(
                    Guid.NewGuid(),
                    Guid.NewGuid()));
    }

    [Test]
    public void GenerateMyPayslip_ShouldThrowBusinessException_WhenUserIsNotOwner()
    {
        var payroll =
            CreateValidPayroll();

        payroll.Employee!.UserId =
            Guid.NewGuid();

        repository
            .Setup(x =>
                x.GetPayrollById(
                    payroll.Id))
            .Returns(
                payroll);

        Assert.Throws<BusinessException>(
            () =>
                service.GenerateMyPayslip(
                    payroll.Id,
                    Guid.NewGuid()));
    }

    private static Payroll
        CreateValidPayroll()
    {
        return new Payroll
        {
            Id = Guid.NewGuid(),

            Employee =
                new Employee
                {
                    FirstName = "John",

                    LastName = "Doe",

                    EmployeeCode = "EMP001",

                    Designation =
                        "Developer",

                    UserId =
                        Guid.NewGuid(),

                    Department =
                        new Department
                        {
                            Name = "IT"
                        }
                },

            PayMonth = 1,

            PayYear = 2025,

            BasicSalary = 50000,

            BasicComponent = 20000,

            HraComponent = 10000,

            SpecialAllowanceComponent =
                8000,

            MedicalAllowanceComponent =
                3000,

            TravelAllowanceComponent =
                2000,

            Bonus = 5000,

            Deductions = 1000,

            LopDeduction = 500,

            NetSalary = 47000,

            GeneratedAt =
                DateTime.UtcNow,

            Status = "Paid"
        };
    }
}