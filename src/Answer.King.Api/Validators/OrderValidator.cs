using Answer.King.Api.RequestModels;
using FluentValidation;

namespace Answer.King.Api.Validators;

public class OrderValidator : AbstractValidator<Order>
{
    public OrderValidator()
    {
        this.RuleForEach(o => o.LineItems)
            .NotNull()
            .SetValidator(new LineItemValidator());
    }
}
