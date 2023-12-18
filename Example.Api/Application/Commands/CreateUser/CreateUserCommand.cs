using Example.Domain;
using Example.Domain.Entities;
using Example.Domain.Repositories;
using Example.Domain.ValueObjects;
using MediatR;

namespace Example.Api.Application.Commands.CreateUser;

internal sealed record CreateUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string CountryCode,
    string City,
    string Street,
    string Building
) : IRequest<UserId>;

internal sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserId>
{
    private readonly IUserRepository _repository;

    public CreateUserCommandHandler(IUserRepository repository) =>
        _repository = repository;

    public async Task<UserId> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var name = Name.Create(request.FirstName, request.LastName);
        var address = new Address(request.CountryCode, request.City, request.Street, request.Building);
        var user = User.Create(request.Email, name, address);

        await _repository.CreateAsync(user, ct);
        await _repository.UnitOfWork.SaveEntitiesAsync(ct);

        return user.Id;
    }
}
