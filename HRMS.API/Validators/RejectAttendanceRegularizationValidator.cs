using FluentValidation;
using HRMS.API.Models.DTOs.AttendanceRegularization;

namespace HRMS.API.Validators;

public class RejectAttendanceRegularizationValidator
    : AbstractValidator<RejectAttendanceRegularizationDto>
{
    public RejectAttendanceRegularizationValidator()
    {
        RuleFor(x => x.HrComments)
            .NotEmpty()
            .MaximumLength(1000);
    }
}