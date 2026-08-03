using ecommerceAPI.BLL.DTOs.Request;
using FluentValidation;

namespace ecommerceAPI.BLL.Validators;

public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
{
    public CreateReviewRequestValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(500).WithMessage("Comment must not exceed 500 characters.");
    }
}
