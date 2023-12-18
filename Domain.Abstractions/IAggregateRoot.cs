namespace Domain.Abstractions;

public interface IAggregateRoot : IEntity
{
    public IReadOnlyCollection<IDomainEvent> GetDomainEvents();

    public void ClearDomainEvents();
}

public interface IAggregateRoot<out TKey> : IEntity<TKey>,
    IAggregateRoot where TKey : struct;
