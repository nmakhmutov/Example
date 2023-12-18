using Example.Domain;
using Example.Domain.Exceptions;
using Example.Domain.Repositories;
using MediatR;

namespace Example.Api.Application.Commands.AddBackCard;

internal sealed record AddBackCardCommand(UserId Id, string CardNumber, string CardName, DateOnly Expiry) : IRequest;

internal sealed class AddBackCardCommandHandler : IRequestHandler<AddBackCardCommand>
{
    private readonly IUserRepository _repository;

    public AddBackCardCommandHandler(IUserRepository repository) =>
        _repository = repository;

    public async Task Handle(AddBackCardCommand request, CancellationToken ct)
    {
        var user = await _repository.GetAsync(request.Id, ct)
                   ?? throw new UserException($"User {request.Id} not found");

        user.AddCard(request.CardNumber, request.CardName, request.Expiry);

        await _repository.UnitOfWork.SaveEntitiesAsync(ct);
    }
}
