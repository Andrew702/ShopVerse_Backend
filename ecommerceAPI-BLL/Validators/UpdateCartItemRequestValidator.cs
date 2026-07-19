using ecommerceAPI.BLL.DTOs.Request;
using FluentValidation;

namespace ecommerceAPI.BLL.Validators;

public class UpdateCartItemRequestValidator : AbstractValidator<UpdateCartItemRequest>
{
    public UpdateCartItemRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .InclusiveBetween(1, 100).WithMessage("Quantity must be between 1 and 100.");
    }
}
