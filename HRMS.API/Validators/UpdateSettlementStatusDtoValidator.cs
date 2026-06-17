using FluentValidation;
using HRMS.API.Models.DTOs.Resignation;

namespace HRMS.API.Validators.EmployeeResignations;

public class UpdateSettlementStatusDtoValidator: AbstractValidator<UpdateSettlementStatusDto>
{
    public UpdateSettlementStatusDtoValidator()
    {
        RuleFor(x => x.FinalSettlementStatus)
            .NotEmpty()
            .WithMessage("Settlement status is required.")
            .Must(status =>
                status == "Pending" ||
                status == "Processing" ||
                status == "Completed")
            .WithMessage(
                "Settlement status must be Pending, Processing or Completed.");
    }
}