namespace Domain.Abstractions;

public abstract class AggregateRoot<TKey> : Entity<TKey>,
    IAggregateRoot<TKey>
    where TKey : struct
{
    private List<IDomainEvent>? _domainEvents;

    protected AggregateRoot(TKey id)
        : base(id) =>
        Version = uint.MinValue;

    public uint Version { get; private set; }

    public IReadOnlyCollection<IDomainEvent> GetDomainEvents() =>
        _domainEvents?.ToArray() ?? [];

    public void ClearDomainEvents() =>
        _domainEvents?.Clear();

    protected void AddDomainEvent(IDomainEvent evt) =>
        (_domainEvents ??= []).Add(evt);

    public uint IncrementVersion()
    {
        if (uint.MaxValue > Version)
            return Version += 1;

        return Version = uint.MinValue + 1;
    }

    public bool IsTransient() =>
        Version == uint.MinValue;

    public override string ToString() =>
        $"Aggregate {{ {GetType().Name} = {Id} }}";
}
