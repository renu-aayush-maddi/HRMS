using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.Payroll;

namespace HRMS.Tests.Services;

[TestFixture]
public class PayrollServiceTests
{
    private Mock<IPayrollRepository>
        repository;

    private Mock<INotificationService>
        notificationService;

    private PayrollService
        service;

    [SetUp]
    public void Setup()
    {
        repository =
            new Mock<IPayrollRepository>();

        notificationService =
            new Mock<INotificationService>();

        service =
            new PayrollService(
                repository.Object,
                notificationService.Object);
    }

    [Test]
    public void GeneratePayroll_ShouldThrowException_WhenPayrollAlreadyExists()
    {
        repository
            .Setup(x =>
                x.GetPayroll(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
            .Returns(
                new Payroll());

        Assert.Throws<Exception>(
            () =>
                service.GeneratePayroll(
                    new GeneratePayrollDto
                    {
                        EmployeeId =
                            Guid.NewGuid(),

                        PayMonth = 1,

                        PayYear = 2025
                    }));
    }

    [Test]
    public void GeneratePayroll_ShouldThrowException_WhenEmployeeNotFound()
    {
        repository
            .Setup(x =>
                x.GetPayroll(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
            .Returns(
                (Payroll?)null);

        repository
            .Setup(x =>
                x.GetEmployee(
                    It.IsAny<Guid>()))
            .Returns(
                (Employee?)null);

        Assert.Throws<Exception>(
            () =>
                service.GeneratePayroll(
                    new GeneratePayrollDto
                    {
                        EmployeeId =
                            Guid.NewGuid(),

                        PayMonth = 1,

                        PayYear = 2025
                    }));
    }

    [Test]
    public void GeneratePayroll_ShouldThrowException_WhenEmployeeSalaryNotAssigned()
    {
        repository
            .Setup(x =>
                x.GetPayroll(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
            .Returns(
                (Payroll?)null);

        repository
            .Setup(x =>
                x.GetEmployee(
                    It.IsAny<Guid>()))
            .Returns(
                new Employee());

        repository
            .Setup(x =>
                x.GetActiveEmployeeSalary(
                    It.IsAny<Guid>()))
            .Returns(
                (EmployeeSalary?)null);

        Assert.Throws<Exception>(
            () =>
                service.GeneratePayroll(
                    new GeneratePayrollDto
                    {
                        EmployeeId =
                            Guid.NewGuid(),

                        PayMonth = 1,

                        PayYear = 2025
                    }));
    }

    [Test]
    public void GeneratePayroll_ShouldGeneratePayroll_WhenValid()
    {
        var employeeId =
            Guid.NewGuid();

        var employee =
            new Employee
            {
                Id = employeeId,

                UserId =
                    Guid.NewGuid()
            };

        var structure =
            new SalaryStructure
            {
                BasicPercentage = 40,

                HraPercentage = 20,

                SpecialAllowancePercentage = 20,

                MedicalAllowancePercentage = 10,

                TravelAllowancePercentage = 10
            };

        var salary =
            new EmployeeSalary
            {
                AnnualCtc =
                    1200000,

                SalaryStructure =
                    structure
            };

        repository
            .Setup(x =>
                x.GetPayroll(
                    employeeId,
                    1,
                    2025))
            .Returns(
                (Payroll?)null);

        repository
            .Setup(x =>
                x.GetEmployee(
                    employeeId))
            .Returns(employee);

        repository
            .Setup(x =>
                x.GetActiveEmployeeSalary(
                    employeeId))
            .Returns(salary);

        repository
            .Setup(x =>
                x.GetWorkingDays(
                    1,
                    2025))
            .Returns(20);

        repository
            .Setup(x =>
                x.GetPresentDays(
                    employeeId,
                    1,
                    2025))
            .Returns(20);

        repository
            .Setup(x =>
                x.GetApprovedPaidLeaveDays(
                    employeeId,
                    1,
                    2025))
            .Returns(0);

        repository
            .Setup(x =>
                x.GetApprovedLopLeaveDays(
                    employeeId,
                    1,
                    2025))
            .Returns(0);

        repository
            .Setup(x =>
                x.GetApprovedBonusAmount(
                    employeeId,
                    1,
                    2025))
            .Returns(5000);

        repository
            .Setup(x =>
                x.GetApprovedDeductionAmount(
                    employeeId,
                    1,
                    2025))
            .Returns(1000);

        repository
            .Setup(x =>
                x.GetApprovedBonuses(
                    employeeId,
                    1,
                    2025))
            .Returns(
                new List<Bonuse>());

        repository
            .Setup(x =>
                x.GetApprovedDeductions(
                    employeeId,
                    1,
                    2025))
            .Returns(
                new List<Deduction>());

        service.GeneratePayroll(
            new GeneratePayrollDto
            {
                EmployeeId =
                    employeeId,

                PayMonth = 1,

                PayYear = 2025
            });

        repository.Verify(
            x => x.AddPayroll(
                It.IsAny<Payroll>()),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);

        notificationService.Verify(
            x => x.CreateNotification(
                employee.UserId.Value,
                "Payroll Generated",
                It.IsAny<string>()),
            Times.Once);
    }
    [Test]
    public void GeneratePayroll_ShouldMarkApprovedBonusesAsProcessed()
    {
        var employeeId =
            Guid.NewGuid();

        var bonuses =
            new List<Bonuse>
            {
                new Bonuse
                {
                    IsProcessed = false
                },
                new Bonuse
                {
                    IsProcessed = false
                }
            };

        SetupValidPayrollGeneration(
            employeeId);

        repository
            .Setup(x =>
                x.GetApprovedBonuses(
                    employeeId,
                    1,
                    2025))
            .Returns(
                bonuses);

        service.GeneratePayroll(
            new GeneratePayrollDto
            {
                EmployeeId =
                    employeeId,

                PayMonth = 1,

                PayYear = 2025
            });

        Assert.That(
            bonuses.All(
                x => x.IsProcessed == true),
            Is.True);
    }

    [Test]
    public void GeneratePayroll_ShouldMarkApprovedDeductionsAsProcessed()
    {
        var employeeId =
            Guid.NewGuid();

        var deductions =
            new List<Deduction>
            {
                new Deduction
                {
                    IsProcessed = false
                },
                new Deduction
                {
                    IsProcessed = false
                }
            };

        SetupValidPayrollGeneration(
            employeeId);

        repository
            .Setup(x =>
                x.GetApprovedDeductions(
                    employeeId,
                    1,
                    2025))
            .Returns(
                deductions);

        service.GeneratePayroll(
            new GeneratePayrollDto
            {
                EmployeeId =
                    employeeId,

                PayMonth = 1,

                PayYear = 2025
            });

        Assert.That(
            deductions.All(
                x => x.IsProcessed == true),
            Is.True);
    }

    [Test]
    public void GeneratePayroll_ShouldCalculateLopDaysCorrectly()
    {
        var employeeId =
            Guid.NewGuid();

        Payroll? capturedPayroll =
            null;

        SetupValidPayrollGeneration(
            employeeId);

        repository
            .Setup(x =>
                x.GetWorkingDays(
                    1,
                    2025))
            .Returns(20);

        repository
            .Setup(x =>
                x.GetPresentDays(
                    employeeId,
                    1,
                    2025))
            .Returns(15);

        repository
            .Setup(x =>
                x.GetApprovedPaidLeaveDays(
                    employeeId,
                    1,
                    2025))
            .Returns(2);

        repository
            .Setup(x =>
                x.GetApprovedLopLeaveDays(
                    employeeId,
                    1,
                    2025))
            .Returns(1);

        repository
            .Setup(x =>
                x.AddPayroll(
                    It.IsAny<Payroll>()))
            .Callback<Payroll>(
                p => capturedPayroll = p);

        service.GeneratePayroll(
            new GeneratePayrollDto
            {
                EmployeeId =
                    employeeId,

                PayMonth = 1,

                PayYear = 2025
            });

        Assert.That(
            capturedPayroll,
            Is.Not.Null);

        Assert.That(
            capturedPayroll!.LopDays,
            Is.EqualTo(4));
    }

    [Test]
    public void GeneratePayroll_ShouldHandleZeroWorkingDays()
    {
        var employeeId =
            Guid.NewGuid();

        Payroll? capturedPayroll =
            null;

        SetupValidPayrollGeneration(
            employeeId);

        repository
            .Setup(x =>
                x.GetWorkingDays(
                    1,
                    2025))
            .Returns(0);

        repository
            .Setup(x =>
                x.AddPayroll(
                    It.IsAny<Payroll>()))
            .Callback<Payroll>(
                p => capturedPayroll = p);

        service.GeneratePayroll(
            new GeneratePayrollDto
            {
                EmployeeId =
                    employeeId,

                PayMonth = 1,

                PayYear = 2025
            });

        Assert.That(
            capturedPayroll,
            Is.Not.Null);

        Assert.That(
            capturedPayroll!.LopDeduction,
            Is.EqualTo(0));
    }

    private void SetupValidPayrollGeneration(
        Guid employeeId)
    {
        repository
            .Setup(x =>
                x.GetPayroll(
                    employeeId,
                    1,
                    2025))
            .Returns(
                (Payroll?)null);

        repository
            .Setup(x =>
                x.GetEmployee(
                    employeeId))
            .Returns(
                new Employee
                {
                    Id = employeeId,
                    UserId = Guid.NewGuid()
                });

        repository
            .Setup(x =>
                x.GetActiveEmployeeSalary(
                    employeeId))
            .Returns(
                new EmployeeSalary
                {
                    AnnualCtc = 1200000,

                    SalaryStructure =
                        new SalaryStructure
                        {
                            BasicPercentage = 40,
                            HraPercentage = 20,
                            SpecialAllowancePercentage = 20,
                            MedicalAllowancePercentage = 10,
                            TravelAllowancePercentage = 10
                        }
                });

        repository
            .Setup(x =>
                x.GetWorkingDays(
                    1,
                    2025))
            .Returns(20);

        repository
            .Setup(x =>
                x.GetPresentDays(
                    employeeId,
                    1,
                    2025))
            .Returns(20);

        repository
            .Setup(x =>
                x.GetApprovedPaidLeaveDays(
                    employeeId,
                    1,
                    2025))
            .Returns(0);

        repository
            .Setup(x =>
                x.GetApprovedLopLeaveDays(
                    employeeId,
                    1,
                    2025))
            .Returns(0);

        repository
            .Setup(x =>
                x.GetApprovedBonusAmount(
                    employeeId,
                    1,
                    2025))
            .Returns(0);

        repository
            .Setup(x =>
                x.GetApprovedDeductionAmount(
                    employeeId,
                    1,
                    2025))
            .Returns(0);

        repository
            .Setup(x =>
                x.GetApprovedBonuses(
                    employeeId,
                    1,
                    2025))
            .Returns(
                new List<Bonuse>());

        repository
            .Setup(x =>
                x.GetApprovedDeductions(
                    employeeId,
                    1,
                    2025))
            .Returns(
                new List<Deduction>());
    }

        [Test]
    public void ApprovePayroll_ShouldApprovePayroll_WhenValid()
    {
        var payroll =
            new Payroll
            {
                Id = Guid.NewGuid(),

                Status = "Generated",

                PayMonth = 1,

                PayYear = 2025,

                Employee =
                    new Employee
                    {
                        UserId =
                            Guid.NewGuid()
                    }
            };

        repository
            .Setup(x =>
                x.GetPayrollById(
                    payroll.Id))
            .Returns(payroll);

        service.ApprovePayroll(
            payroll.Id);

        Assert.That(
            payroll.Status,
            Is.EqualTo(
                "Approved"));

        repository.Verify(
            x => x.UpdatePayroll(
                payroll),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void ApprovePayroll_ShouldThrowException_WhenPayrollNotFound()
    {
        repository
            .Setup(x =>
                x.GetPayrollById(
                    It.IsAny<Guid>()))
            .Returns(
                (Payroll?)null);

        Assert.Throws<Exception>(
            () =>
                service.ApprovePayroll(
                    Guid.NewGuid()));
    }

    [Test]
    public void ApprovePayroll_ShouldThrowException_WhenPayrollStatusIsNotGenerated()
    {
        repository
            .Setup(x =>
                x.GetPayrollById(
                    It.IsAny<Guid>()))
            .Returns(
                new Payroll
                {
                    Status = "Approved"
                });

        Assert.Throws<Exception>(
            () =>
                service.ApprovePayroll(
                    Guid.NewGuid()));
    }

    [Test]
    public void ApprovePayroll_ShouldCreateNotification()
    {
        var userId =
            Guid.NewGuid();

        var payroll =
            new Payroll
            {
                Id = Guid.NewGuid(),

                Status = "Generated",

                PayMonth = 1,

                PayYear = 2025,

                Employee =
                    new Employee
                    {
                        UserId = userId
                    }
            };

        repository
            .Setup(x =>
                x.GetPayrollById(
                    payroll.Id))
            .Returns(
                payroll);

        service.ApprovePayroll(
            payroll.Id);

        notificationService.Verify(
            x =>
                x.CreateNotification(
                    userId,
                    "Payroll Approved",
                    It.IsAny<string>()),
            Times.Once);
    }

    [Test]
    public void MarkPayrollPaid_ShouldMarkPayrollPaid_WhenValid()
    {
        var payroll =
            new Payroll
            {
                Id = Guid.NewGuid(),

                Status = "Approved",

                Employee =
                    new Employee()
            };

        repository
            .Setup(x =>
                x.GetPayrollById(
                    payroll.Id))
            .Returns(
                payroll);

        service.MarkPayrollPaid(
            payroll.Id);

        Assert.That(
            payroll.Status,
            Is.EqualTo(
                "Paid"));

        repository.Verify(
            x => x.UpdatePayroll(
                payroll),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void MarkPayrollPaid_ShouldThrowException_WhenPayrollNotFound()
    {
        repository
            .Setup(x =>
                x.GetPayrollById(
                    It.IsAny<Guid>()))
            .Returns(
                (Payroll?)null);

        Assert.Throws<Exception>(
            () =>
                service.MarkPayrollPaid(
                    Guid.NewGuid()));
    }

    [Test]
    public void MarkPayrollPaid_ShouldThrowException_WhenPayrollStatusIsNotApproved()
    {
        repository
            .Setup(x =>
                x.GetPayrollById(
                    It.IsAny<Guid>()))
            .Returns(
                new Payroll
                {
                    Status = "Generated"
                });

        Assert.Throws<Exception>(
            () =>
                service.MarkPayrollPaid(
                    Guid.NewGuid()));
    }

    [Test]
    public void MarkPayrollPaid_ShouldCreateNotification()
    {
        var userId =
            Guid.NewGuid();

        var payroll =
            new Payroll
            {
                Id = Guid.NewGuid(),

                Status = "Approved",

                PayMonth = 1,

                PayYear = 2025,

                Employee =
                    new Employee
                    {
                        UserId = userId
                    }
            };

        repository
            .Setup(x =>
                x.GetPayrollById(
                    payroll.Id))
            .Returns(
                payroll);

        service.MarkPayrollPaid(
            payroll.Id);

        notificationService.Verify(
            x =>
                x.CreateNotification(
                    userId,
                    "Salary Credited",
                    It.IsAny<string>()),
            Times.Once);
    }

        [Test]
    public void GetAllPayrolls_ShouldReturnMappedDtos()
    {
        repository
            .Setup(x =>
                x.GetAllPayrolls())
            .Returns(
                new List<Payroll>
                {
                    new Payroll
                    {
                        Id = Guid.NewGuid(),

                        PayMonth = 1,

                        PayYear = 2025,

                        BasicSalary = 50000,

                        NetSalary = 55000,

                        Employee =
                            new Employee
                            {
                                FirstName = "John",
                                LastName = "Doe"
                            }
                    }
                });

        var result =
            service.GetAllPayrolls();

        Assert.That(
            result.Count,
            Is.EqualTo(1));

        Assert.That(
            result[0].EmployeeName,
            Is.EqualTo("John Doe"));

        Assert.That(
            result[0].BasicSalary,
            Is.EqualTo(50000));
    }

    [Test]
    public void GetEmployeePayrolls_ShouldReturnMappedDtos()
    {
        var employeeId =
            Guid.NewGuid();

        repository
            .Setup(x =>
                x.GetEmployeePayrolls(
                    employeeId))
            .Returns(
                new List<Payroll>
                {
                    new Payroll
                    {
                        Id = Guid.NewGuid(),

                        PayMonth = 1,

                        PayYear = 2025,

                        BasicSalary = 50000,

                        Employee =
                            new Employee
                            {
                                FirstName = "John",
                                LastName = "Doe"
                            }
                    }
                });

        var result =
            service.GetEmployeePayrolls(
                employeeId);

        Assert.That(
            result.Count,
            Is.EqualTo(1));

        Assert.That(
            result[0].EmployeeName,
            Is.EqualTo("John Doe"));
    }

    [Test]
    public void GetMyPayrolls_ShouldReturnMappedDtos()
    {
        var employee =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns(
                employee);

        repository
            .Setup(x =>
                x.GetEmployeePayrolls(
                    employee.Id))
            .Returns(
                new List<Payroll>
                {
                    new Payroll
                    {
                        Id = Guid.NewGuid(),

                        BasicSalary = 50000,

                        Employee =
                            new Employee
                            {
                                FirstName = "John",
                                LastName = "Doe"
                            }
                    }
                });

        var result =
            service.GetMyPayrolls(
                Guid.NewGuid());

        Assert.That(
            result.Count,
            Is.EqualTo(1));

        Assert.That(
            result[0].EmployeeName,
            Is.EqualTo("John Doe"));
    }

    [Test]
    public void GetMyPayrolls_ShouldThrowException_WhenEmployeeNotFound()
    {
        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns(
                (Employee?)null);

        Assert.Throws<Exception>(
            () =>
                service.GetMyPayrolls(
                    Guid.NewGuid()));
    }

    [Test]
    public void GenerateMonthlyPayroll_ShouldReturnCorrectCounts()
    {
        repository
            .Setup(x =>
                x.GetActiveEmployees())
            .Returns(
                new List<Employee>
                {
                    new Employee
                    {
                        Id = Guid.NewGuid(),
                        EmployeeCode = "EMP001"
                    },
                    new Employee
                    {
                        Id = Guid.NewGuid(),
                        EmployeeCode = "EMP002"
                    }
                });

        repository
            .Setup(x =>
                x.GetPayroll(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
            .Returns(
                new Payroll());

        var result =
            service.GenerateMonthlyPayroll(
                new GenerateMonthlyPayrollDto
                {
                    PayMonth = 1,
                    PayYear = 2025
                });

        Assert.That(
            result.TotalEmployees,
            Is.EqualTo(2));

        Assert.That(
            result.SuccessCount,
            Is.EqualTo(0));

        Assert.That(
            result.FailedCount,
            Is.EqualTo(2));
    }

    [Test]
    public void GenerateMonthlyPayroll_ShouldCaptureEmployeeErrors()
    {
        repository
            .Setup(x =>
                x.GetActiveEmployees())
            .Returns(
                new List<Employee>
                {
                    new Employee
                    {
                        Id = Guid.NewGuid(),
                        EmployeeCode = "EMP001"
                    }
                });

        repository
            .Setup(x =>
                x.GetPayroll(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
            .Returns(
                new Payroll());

        var result =
            service.GenerateMonthlyPayroll(
                new GenerateMonthlyPayrollDto
                {
                    PayMonth = 1,
                    PayYear = 2025
                });

        Assert.That(
            result.Errors.Count,
            Is.EqualTo(1));

        Assert.That(
            result.Errors[0],
            Does.Contain("EMP001"));
    }

    [Test]
    public void GenerateMonthlyPayroll_ShouldGeneratePayrollForAllEmployees()
    {
        var employees =
            new List<Employee>
            {
                new Employee
                {
                    Id = Guid.NewGuid(),
                    EmployeeCode = "EMP001"
                },
                new Employee
                {
                    Id = Guid.NewGuid(),
                    EmployeeCode = "EMP002"
                }
            };

        repository
            .Setup(x =>
                x.GetActiveEmployees())
            .Returns(
                employees);

        foreach (var employee in employees)
        {
            repository
                .Setup(x =>
                    x.GetPayroll(
                        employee.Id,
                        1,
                        2025))
                .Returns(
                    (Payroll?)null);

            repository
                .Setup(x =>
                    x.GetEmployee(
                        employee.Id))
                .Returns(
                    new Employee
                    {
                        Id = employee.Id,
                        UserId = Guid.NewGuid()
                    });

            repository
                .Setup(x =>
                    x.GetActiveEmployeeSalary(
                        employee.Id))
                .Returns(
                    new EmployeeSalary
                    {
                        AnnualCtc = 1200000,

                        SalaryStructure =
                            new SalaryStructure
                            {
                                BasicPercentage = 40,
                                HraPercentage = 20,
                                SpecialAllowancePercentage = 20,
                                MedicalAllowancePercentage = 10,
                                TravelAllowancePercentage = 10
                            }
                    });

            repository
                .Setup(x =>
                    x.GetWorkingDays(
                        1,
                        2025))
                .Returns(20);

            repository
                .Setup(x =>
                    x.GetPresentDays(
                        employee.Id,
                        1,
                        2025))
                .Returns(20);

            repository
                .Setup(x =>
                    x.GetApprovedPaidLeaveDays(
                        employee.Id,
                        1,
                        2025))
                .Returns(0);

            repository
                .Setup(x =>
                    x.GetApprovedLopLeaveDays(
                        employee.Id,
                        1,
                        2025))
                .Returns(0);

            repository
                .Setup(x =>
                    x.GetApprovedBonusAmount(
                        employee.Id,
                        1,
                        2025))
                .Returns(0);

            repository
                .Setup(x =>
                    x.GetApprovedDeductionAmount(
                        employee.Id,
                        1,
                        2025))
                .Returns(0);

            repository
                .Setup(x =>
                    x.GetApprovedBonuses(
                        employee.Id,
                        1,
                        2025))
                .Returns(
                    new List<Bonuse>());

            repository
                .Setup(x =>
                    x.GetApprovedDeductions(
                        employee.Id,
                        1,
                        2025))
                .Returns(
                    new List<Deduction>());
        }

        var result =
            service.GenerateMonthlyPayroll(
                new GenerateMonthlyPayrollDto
                {
                    PayMonth = 1,
                    PayYear = 2025
                });

        Assert.That(
            result.TotalEmployees,
            Is.EqualTo(2));

        Assert.That(
            result.SuccessCount,
            Is.EqualTo(2));

        Assert.That(
            result.FailedCount,
            Is.EqualTo(0));
    }
}