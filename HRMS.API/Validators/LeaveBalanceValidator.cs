using HRMS.API.Exceptions;
using HRMS.API.Models.DTOs.LeaveBalance;

namespace HRMS.API.Validators;

public class LeaveBalanceValidator
{
    public Task ValidateAllocationAsync(
        AllocateLeaveBalanceDto dto)
    {
        if (dto.AllocatedDays <= 0)
        {
            throw new ValidationException(
                "Allocated days must be greater than zero");
        }

        return Task.CompletedTask;
    }
}