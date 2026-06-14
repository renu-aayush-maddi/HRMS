using HRMS.API.Exceptions;
using HRMS.API.Models.DTOs.Leave;

namespace HRMS.API.Validators;

public class LeaveValidator
{
    public Task ValidateApplyLeaveAsync(
        ApplyLeaveDto dto)
    {
        if (dto.FromDate > dto.ToDate)
        {
            throw new ValidationException(
                "From date cannot be greater than To date");
        }

        if (dto.FromDate <
            DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new BusinessException(
                "Cannot apply leave for past dates");
        }

        if (string.IsNullOrWhiteSpace(dto.Reason))
        {
            throw new ValidationException(
                "Reason is required");
        }

        return Task.CompletedTask;
    }
}