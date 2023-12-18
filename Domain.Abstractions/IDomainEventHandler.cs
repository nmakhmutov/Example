using MediatR;

namespace Domain.Abstractions;

public interface IDomainEventHandler<in T> : INotificationHandler<T> where T : IDomainEvent;
