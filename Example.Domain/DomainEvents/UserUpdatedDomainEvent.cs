using Domain.Abstractions;

namespace Example.Domain.DomainEvents;

public sealed record UserUpdatedDomainEvent : IDomainEvent
{
    public UserUpdatedDomainEvent(Entities.User user) =>
        User = user;

    public Entities.User User { get; }
}
