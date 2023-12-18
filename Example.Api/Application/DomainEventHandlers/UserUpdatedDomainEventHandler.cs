using Domain.Abstractions;
using Example.Domain.DomainEvents;

namespace Example.Api.Application.DomainEventHandlers;

internal class UserUpdatedDomainEventHandler : IDomainEventHandler<UserUpdatedDomainEvent>
{
    private readonly ILogger<UserUpdatedDomainEventHandler> _logger;

    public UserUpdatedDomainEventHandler(ILogger<UserUpdatedDomainEventHandler> logger) =>
        _logger = logger;

    public Task Handle(UserUpdatedDomainEvent notification, CancellationToken ct)
    {
        var divider = string.Concat(Enumerable.Repeat('-', 100));
        _logger.LogInformation("{d} User updated: {Id}", divider, notification.User.Id);

        return Task.CompletedTask;
    }
}
