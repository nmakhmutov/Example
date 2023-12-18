using Example.Domain.ValueObjects;
using FluentValidation;

namespace Example.Api.Application.Commands.CreateUser;

internal sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MinimumLength(Name.MinFirstNameLen)
            .MaximumLength(Name.MaxFirstNameLen);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MinimumLength(Name.MinLastNameLen)
            .MaximumLength(Name.MaxLastNameLen);

        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .Length(2);

        RuleFor(x => x.City)
            .NotEmpty();

        RuleFor(x => x.Street)
            .NotEmpty();

        RuleFor(x => x.Building)
            .NotEmpty();
    }
}
