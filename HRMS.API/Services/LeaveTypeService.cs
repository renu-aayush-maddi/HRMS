using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.LeaveType;
using HRMS.API.Models.Entities;
using HRMS.API.Exceptions;

namespace HRMS.API.Services;

public class LeaveTypeService: ILeaveTypeService
{
    private readonly ILeaveTypeRepository repository;

    public LeaveTypeService(ILeaveTypeRepository repository)
    {
        this.repository = repository;
    }

    public List<LeaveTypeResponseDto> GetAll()
    {
        return repository
            .GetAll()
            .Select(x =>
                new LeaveTypeResponseDto
                {
                    Id = x.Id,

                    Name = x.Name,

                    AnnualAllocation =
                        x.AnnualAllocation,

                    CarryForwardAllowed =
                        x.CarryForwardAllowed ?? false,

                    MaxCarryForward =
                        x.MaxCarryForward ?? 0,

                    NegativeBalanceAllowed =
                        x.NegativeBalanceAllowed ?? false,

                    IsActive =
                        x.IsActive ?? false
                })
            .ToList();
    }

    public void Add(AddLeaveTypeDto dto)
    {
        if(repository.Exists(dto.Name))
        {
            throw new BusinessException("Leave type already exists");
        }

        if(!dto.CarryForwardAllowed && dto.MaxCarryForward > 0)
            {
                throw new BusinessException("Max carry forward must be zero when carry forward is disabled");
            }
        if(dto.AnnualAllocation < 0)
            {
                throw new BusinessException(
                    "Invalid annual allocation");
            }

        LeaveType leaveType = new LeaveType
            {
                Id = Guid.NewGuid(),

                Name = dto.Name,

                AnnualAllocation =
                    dto.AnnualAllocation,

                CarryForwardAllowed =
                    dto.CarryForwardAllowed,

                MaxCarryForward =
                    dto.MaxCarryForward,

                NegativeBalanceAllowed =
                    dto.NegativeBalanceAllowed,

                IsActive = true
            };

        repository.Add(leaveType);

        repository.SaveChanges();
    }

    public void Update(Guid id,UpdateLeaveTypeDto dto)
    {
        var leaveType = repository.GetById(id);

        if(leaveType == null)
        {
            throw new NotFoundException("Leave type not found");
        }

        if(!dto.CarryForwardAllowed && dto.MaxCarryForward > 0)
            {
                throw new BusinessException("Max carry forward must be zero when carry forward is disabled");
            }
        if(dto.AnnualAllocation < 0)
            {
                throw new BusinessException("Invalid annual allocation");
            }
        
        if(leaveType.Name.ToLower()!= dto.Name.ToLower() && repository.Exists(dto.Name))
        {
            throw new BusinessException("Leave type already exists");
        }
        

        leaveType.Name = dto.Name;

        leaveType.AnnualAllocation =
            dto.AnnualAllocation;

        leaveType.CarryForwardAllowed =
            dto.CarryForwardAllowed;

        leaveType.MaxCarryForward =
            dto.MaxCarryForward;

        leaveType.NegativeBalanceAllowed =
            dto.NegativeBalanceAllowed;

        leaveType.IsActive =
            dto.IsActive;

        repository.Update(leaveType);

        repository.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var leaveType = repository.GetById(id);

        if(leaveType == null)
        {
            throw new NotFoundException("Leave type not found");
        }

        if(repository.IsUsed(id))
        {
            throw new BusinessException("Cannot delete leave type that is in use");
        }

        repository.Delete(leaveType);

        repository.SaveChanges();
    }
}