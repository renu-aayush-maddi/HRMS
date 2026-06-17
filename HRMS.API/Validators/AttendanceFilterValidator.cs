using FluentValidation;
using HRMS.API.Common.Constants;
using HRMS.API.Models.DTOs.Attendance;

namespace HRMS.API.Validators;

public class AttendanceFilterValidator : AbstractValidator<AttendanceFilterDto>
{
    public AttendanceFilterValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x)
            .Must(x => !x.FromDate.HasValue || !x.ToDate.HasValue || x.FromDate <= x.ToDate)
            .WithMessage("FromDate cannot be greater than ToDate.");

        RuleFor(x => x.Status)
            .Must(x => string.IsNullOrWhiteSpace(x) || AttendanceConstants.All.Contains(x))
            .WithMessage($"Status must be one of: {string.Join(", ", AttendanceConstants.All)}");
    }
}