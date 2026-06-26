using FluentValidation;

public class CreateItemValidator : AbstractValidator<CreateItemRequest>
{
    public CreateItemValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}