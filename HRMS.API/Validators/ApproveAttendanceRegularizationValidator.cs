using FluentValidation;
using HRMS.API.Models.DTOs.AttendanceRegularization;

namespace HRMS.API.Validators;

public class ApproveAttendanceRegularizationValidator: AbstractValidator<ApproveAttendanceRegularizationDto>
{
    public ApproveAttendanceRegularizationValidator()
    {
        RuleFor(x => x.HrComments)
            .MaximumLength(1000);
    }
}