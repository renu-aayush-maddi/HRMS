using FluentValidation;
using HRMS.API.Models.DTOs.AttendanceRegularization;

namespace HRMS.API.Validators;

public class CreateAttendanceRegularizationValidator
    : AbstractValidator<CreateAttendanceRegularizationDto>
{
    public CreateAttendanceRegularizationValidator()
    {
        RuleFor(x => x.AttendanceDate)
            .NotEmpty();

        RuleFor(x => x.RequestedCheckIn)
            .NotEmpty();

        RuleFor(x => x.RequestedCheckOut)
            .NotEmpty();

        RuleFor(x => x)
            .Must(x => x.RequestedCheckIn < x.RequestedCheckOut)
            .WithMessage(
                "RequestedCheckIn must be earlier than RequestedCheckOut.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(1000);
    }
}