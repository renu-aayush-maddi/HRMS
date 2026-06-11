using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Bonus;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

public class BonusService : IBonusService
{
    private readonly IBonusRepository bonusRepository;
    private readonly INotificationService notificationService;

    public BonusService(
        IBonusRepository bonusRepository,
        INotificationService notificationService)
    {
        this.bonusRepository =
            bonusRepository;

        this.notificationService =
            notificationService;
    }

    public void CreateBonus(
        CreateBonusDto dto)
    {
        var employee =
            bonusRepository
            .GetEmployee(
                dto.EmployeeId);

        if (employee == null)
        {
            throw new NotFoundException(
                "Employee not found");
        }

        Bonuse bonus = new()
        {
            Id = Guid.NewGuid(),

            EmployeeId =
                dto.EmployeeId,

            Amount =
                dto.Amount,

            Reason =
                dto.Reason,

            BonusMonth =
                dto.BonusMonth,

            BonusYear =
                dto.BonusYear,

            Status =
                "Pending"
        };

        bonusRepository
            .AddBonus(bonus);

        bonusRepository
            .SaveChanges();
    }

    public void ApproveBonus(
        Guid bonusId)
    {
        var bonus =
            bonusRepository
            .GetBonus(bonusId);

        if (bonus == null)
        {
            throw new NotFoundException(
                "Bonus not found");
        }

        if (bonus.Status != "Pending")
        {
            throw new BusinessException(
                "Bonus already processed");
        }

        bonus.Status = "Approved";

        bonusRepository
            .UpdateBonus(bonus);

        bonusRepository
            .SaveChanges();

        if (bonus.Employee.UserId != null)
        {
            notificationService
                .CreateNotification(
                    bonus.Employee.UserId.Value,
                    "Bonus Approved",
                    $"Bonus of {bonus.Amount} approved.");
        }
    }

    public void RejectBonus(
        Guid bonusId)
    {
        var bonus =
            bonusRepository
            .GetBonus(bonusId);

        if (bonus == null)
        {
            throw new NotFoundException(
                "Bonus not found");
        }

        if (bonus.Status != "Pending")
        {
            throw new BusinessException(
                "Bonus already processed");
        }

        bonus.Status = "Rejected";

        bonusRepository
            .UpdateBonus(bonus);

        bonusRepository
            .SaveChanges();
    }

    public List<BonusResponseDto>
        GetAllBonuses()
    {
        return bonusRepository
            .GetAllBonuses()
            .Select(x =>
                new BonusResponseDto
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

                    BonusMonth =
                        x.BonusMonth,

                    BonusYear =
                        x.BonusYear,

                    Status =
                        x.Status ?? ""
                })
            .ToList();
    }

    public List<BonusResponseDto>
        GetMyBonuses(Guid userId)
    {
        var employee =
            bonusRepository
            .GetEmployeeByUserId(
                userId);

        if (employee == null)
        {
            throw new NotFoundException(
                "Employee not found");
        }

        return bonusRepository
            .GetEmployeeBonuses(
                employee.Id)
            .Select(x =>
                new BonusResponseDto
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

                    BonusMonth =
                        x.BonusMonth,

                    BonusYear =
                        x.BonusYear,

                    Status =
                        x.Status ?? ""
                })
            .ToList();
    }
}