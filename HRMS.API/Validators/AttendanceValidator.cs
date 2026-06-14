using HRMS.API.Exceptions;
using HRMS.API.Models.DTOs.Attendance;

namespace HRMS.API.Validators;

public class AttendanceValidator
{
    private static readonly HashSet<string> ValidStatuses =
    [
        "Present",
        "Absent",
        "Late",
        "HalfDay"
    ];

    public Task ValidateQueryAsync(
        AttendanceQueryDto query)
    {
        if (query.Page <= 0)
        {
            throw new ValidationException(
                "Page must be greater than zero");
        }

        if (query.PageSize <= 0)
        {
            throw new ValidationException(
                "PageSize must be greater than zero");
        }

        if (query.FromDate.HasValue &&
            query.ToDate.HasValue &&
            query.FromDate > query.ToDate)
        {
            throw new ValidationException(
                "FromDate cannot be greater than ToDate");
        }

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            !ValidStatuses.Contains(query.Status))
        {
            throw new ValidationException(
                "Invalid attendance status");
        }

        return Task.CompletedTask;
    }
}