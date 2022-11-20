using Answer.King.Api.RequestModels;
using FluentValidation;

namespace Answer.King.Api.Validators;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        this.RuleFor(p => p.Name)
            .NotNullOrWhiteSpace();

        this.RuleFor(p => p.Description)
            .NotNullOrWhiteSpace();

        this.RuleFor(p => p.Price)
            .GreaterThanOrEqualTo(0.00);

        this.RuleFor(p => p.Categories)
            .NotEmpty()
            .ForEach(p =>
                p.NotNull()
                    .GreaterThanOrEqualTo(0));
    }
}
