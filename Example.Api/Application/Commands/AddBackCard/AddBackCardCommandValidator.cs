using FluentValidation;

namespace Example.Api.Application.Commands.AddBackCard;

internal sealed class AddBackCardCommandValidator : AbstractValidator<AddBackCardCommand>
{
    public AddBackCardCommandValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .Must(x => x.All(c => char.IsDigit(c)))
            .WithMessage("Card must contains only digits");
        
        RuleFor(x => x.CardName)
            .NotEmpty();

        RuleFor(x => x.Expiry)
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow));
    }
}
