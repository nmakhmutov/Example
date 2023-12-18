using Domain.Abstractions;
using Example.Domain.Entities;

namespace Example.Domain.DomainEvents;

public sealed record BankCardAddedDomainEvent : IDomainEvent
{
    public BankCardAddedDomainEvent(User user, BankCard card)
    {
        User = user;
        Card = card;
    }

    public User User { get; }

    public BankCard Card { get; }
}
