using Domain.Abstractions;
using Example.Domain.DomainEvents;

namespace Example.Api.Application.DomainEventHandlers;

internal class UserCreatedDomainEventHandler : IDomainEventHandler<UserCreatedDomainEvent>
{
    private readonly ILogger<UserCreatedDomainEventHandler> _logger;

    public UserCreatedDomainEventHandler(ILogger<UserCreatedDomainEventHandler> logger) =>
        _logger = logger;

    public Task Handle(UserCreatedDomainEvent notification, CancellationToken ct)
    {
        var divider = string.Concat(Enumerable.Repeat('-', 100));
        _logger.LogInformation("{d} User created: {Id}", divider, notification.User.Id);

        return Task.CompletedTask;
    }
}
