using Domain.Abstractions;
using Example.Domain.Entities;

namespace Example.Domain.DomainEvents;

public sealed record UserCreatedDomainEvent(User User) : IDomainEvent;
