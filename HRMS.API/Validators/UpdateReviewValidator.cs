using FluentValidation;
using HRMS.API.Models.DTOs.Review;

namespace HRMS.API.Validators;

public class UpdateReviewValidator : AbstractValidator<UpdateReviewDto>
{
    public UpdateReviewValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5);

        RuleFor(x => x.Comments)
            .MaximumLength(2000);
    }
}