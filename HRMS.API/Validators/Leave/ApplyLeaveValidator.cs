using FluentValidation;
using HRMS.API.Models.DTOs.Leave;

namespace HRMS.API.Validators.Leave;

public class ApplyLeaveValidator
    : AbstractValidator<ApplyLeaveDto>
{
    public ApplyLeaveValidator()
    {
        RuleFor(x => x.LeaveTypeId)
            .NotEmpty();

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate);

        RuleFor(x => x.FromDate)
            .GreaterThanOrEqualTo(
                DateOnly.FromDateTime(DateTime.Today));
    }
}