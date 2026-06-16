using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Resignation;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

public class ResignationService
    : IResignationService
{
    private readonly IResignationRepository repository;

    public ResignationService(IResignationRepository repository)
    {
        this.repository = repository;
    }

    public void SubmitResignation(SubmitResignationDto dto)
    {
        var employee =
            repository.GetEmployee(
                dto.EmployeeId);

        if(employee == null)
        {
            throw new Exception(
                "Employee not found");
        }

        EmployeeResignation resignation =
            new()
            {
                Id = Guid.NewGuid(),

                EmployeeId =
                    dto.EmployeeId,

                ResignationDate =
                    DateOnly.FromDateTime(
                        DateTime.Today),

                LastWorkingDate =
                    dto.LastWorkingDate,

                Reason =
                    dto.Reason,

                Status =
                    "Pending",

                FinalSettlementStatus =
                    "Pending"
            };

        repository.Add(resignation);

        repository.SaveChanges();
    }

    public List<ResignationResponseDto>
        GetAll()
    {
        return repository
            .GetAll()
            .Select(r =>
                new ResignationResponseDto
                {
                    Id = r.Id,

                    EmployeeName =
                        r.Employee!.FirstName +
                        " " +
                        r.Employee.LastName,

                    ResignationDate =
                        r.ResignationDate,

                    LastWorkingDate =
                        r.LastWorkingDate,

                    Reason =
                        r.Reason,

                    Status =
                        r.Status,

                    HrComments =
                        r.HrComments,

                    FinalSettlementStatus =
                        r.FinalSettlementStatus
                })
            .ToList();
    }

    public List<ResignationResponseDto>
        GetEmployeeResignations(
            Guid employeeId)
    {
        return repository
            .GetEmployeeResignations(
                employeeId)
            .Select(r =>
                new ResignationResponseDto
                {
                    Id = r.Id,

                    EmployeeName =
                        r.Employee!.FirstName +
                        " " +
                        r.Employee.LastName,

                    ResignationDate =
                        r.ResignationDate,

                    LastWorkingDate =
                        r.LastWorkingDate,

                    Reason =
                        r.Reason,

                    Status =
                        r.Status,

                    HrComments =
                        r.HrComments,

                    FinalSettlementStatus =
                        r.FinalSettlementStatus
                })
            .ToList();
    }

    public void Approve(
        Guid resignationId,
        ResignationActionDto dto)
    {
        var resignation =
            repository.GetResignation(
                resignationId);

        if(resignation == null)
        {
            throw new Exception(
                "Resignation not found");
        }

        resignation.Status =
            "Approved";

        resignation.HrComments =
            dto.HrComments;

        if(resignation.Employee != null)
        {
            resignation.Employee
                .EmploymentStatus =
                    "NoticePeriod";
        }

        repository.Update(resignation);

        repository.SaveChanges();
    }

    public void Reject(
        Guid resignationId,
        ResignationActionDto dto)
    {
        var resignation = repository.GetResignation(resignationId);

        if(resignation == null)
        {
            throw new Exception("Resignation not found");
        }

        resignation.Status = "Rejected";

        resignation.HrComments = dto.HrComments;

        repository.Update(resignation);

        repository.SaveChanges();
    }

    public void UpdateSettlement(Guid resignationId,SettlementDto dto)
    {
        var resignation =
            repository.GetResignation(
                resignationId);

        if(resignation == null)
        {
            throw new Exception("Resignation not found");
        }

        resignation.FinalSettlementStatus =
            dto.SettlementStatus;

        repository.Update(resignation);

        repository.SaveChanges();
    }
}