using FluentValidation;
using HRMS.API.Models.DTOs.Leave;

namespace HRMS.API.Validators.Leave;

public class LeaveActionValidator
    : AbstractValidator<LeaveActionDto>
{
    public LeaveActionValidator()
    {
        RuleFor(x => x.ManagerComments)
            .MaximumLength(500);
    }
}