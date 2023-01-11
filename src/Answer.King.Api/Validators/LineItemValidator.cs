using Answer.King.Api.RequestModels;
using FluentValidation;

namespace Answer.King.Api.Validators;

public class LineItemValidator : AbstractValidator<LineItem>
{
    public LineItemValidator()
    {
        this.RuleFor(li => li.ProductId)
            .NotEmpty();

        this.RuleFor(li => li.Quantity)
            .GreaterThanOrEqualTo(0);
    }
}
