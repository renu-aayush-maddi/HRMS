using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Deduction;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

public class DeductionService : IDeductionService
{
    private readonly IDeductionRepository deductionRepository;
    private readonly INotificationService notificationService;

    public DeductionService(
        IDeductionRepository deductionRepository,
        INotificationService notificationService)
    {
        this.deductionRepository =
            deductionRepository;

        this.notificationService =
            notificationService;
    }

    public void CreateDeduction(
        CreateDeductionDto dto)
    {
        var employee =
            deductionRepository
            .GetEmployee(dto.EmployeeId);

        if (employee == null)
        {
            throw new NotFoundException(
                "Employee not found");
        }

        Deduction deduction = new()
        {
            Id = Guid.NewGuid(),

            EmployeeId =
                dto.EmployeeId,

            Amount =
                dto.Amount,

            Reason =
                dto.Reason,

            DeductionMonth =
                dto.DeductionMonth,

            DeductionYear =
                dto.DeductionYear,

            Status =
                "Pending"
        };

        deductionRepository
            .AddDeduction(deduction);

        deductionRepository
            .SaveChanges();
    }

    public void ApproveDeduction(
        Guid deductionId)
    {
        var deduction =
            deductionRepository
            .GetDeduction(deductionId);

        if (deduction == null)
        {
            throw new NotFoundException(
                "Deduction not found");
        }

        if (deduction.Status != "Pending")
        {
            throw new BusinessException(
                "Deduction already processed");
        }

        deduction.Status = "Approved";

        deductionRepository
            .UpdateDeduction(deduction);

        deductionRepository
            .SaveChanges();

        if (deduction.Employee.UserId != null)
        {
            notificationService
                .CreateNotification(
                    deduction.Employee.UserId.Value,
                    "Deduction Approved",
                    $"Deduction of {deduction.Amount} approved.");
        }
    }

    public void RejectDeduction(
        Guid deductionId)
    {
        var deduction =
            deductionRepository
            .GetDeduction(deductionId);

        if (deduction == null)
        {
            throw new NotFoundException(
                "Deduction not found");
        }

        if (deduction.Status != "Pending")
        {
            throw new BusinessException(
                "Deduction already processed");
        }

        deduction.Status = "Rejected";

        deductionRepository
            .UpdateDeduction(deduction);

        deductionRepository
            .SaveChanges();

        if (deduction.Employee.UserId != null)
        {
            notificationService
                .CreateNotification(
                    deduction.Employee.UserId.Value,
                    "Deduction Rejected",
                    $"Deduction of {deduction.Amount} rejected.");
        }
    }

    public List<DeductionResponseDto>
        GetAllDeductions()
    {
        return deductionRepository
            .GetAllDeductions()
            .Select(x =>
                new DeductionResponseDto
                {
                    Id = x.Id,

                    EmployeeName =
                        x.Employee.FirstName +
                        " " +
                        x.Employee.LastName,

                    Amount =
                        x.Amount,

                    Reason =
                        x.Reason,

                    DeductionMonth =
                        x.DeductionMonth,

                    DeductionYear =
                        x.DeductionYear,

                    Status =
                        x.Status ?? ""
                })
            .ToList();
    }

    public List<DeductionResponseDto>
        GetMyDeductions(Guid userId)
    {
        var employee =
            deductionRepository
            .GetEmployeeByUserId(userId);

        if (employee == null)
        {
            throw new NotFoundException(
                "Employee not found");
        }

        return deductionRepository
            .GetEmployeeDeductions(
                employee.Id)
            .Select(x =>
                new DeductionResponseDto
                {
                    Id = x.Id,

                    EmployeeName =
                        x.Employee.FirstName +
                        " " +
                        x.Employee.LastName,

                    Amount =
                        x.Amount,

                    Reason =
                        x.Reason,

                    DeductionMonth =
                        x.DeductionMonth,

                    DeductionYear =
                        x.DeductionYear,

                    Status =
                        x.Status ?? ""
                })
            .ToList();
    }
}