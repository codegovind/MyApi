using FluentValidation;
using TaxAccount.DTOs;

namespace TaxAccount.Validators
{
    public class CreateProductValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z0-9\s]+$")
                .WithMessage("Name can only contain letters, numbers and spaces");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0")
                .LessThanOrEqualTo(999999.99m)
                .WithMessage("Price cannot exceed 999999.99");
        }
    }
}