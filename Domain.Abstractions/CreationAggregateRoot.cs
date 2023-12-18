namespace Domain.Abstractions;

public abstract class CreationAggregateRoot<TKey> : AggregateRoot<TKey>,
    ICreated
    where TKey : struct
{
    protected CreationAggregateRoot(TKey id)
        : base(id) =>
        CreatedAt = DateTime.MinValue;

    public DateTime CreatedAt { get; protected set; }

    public virtual void SetAsUpdated(DateTime date)
    {
        if (CreatedAt > DateTime.MinValue)
            return;

        CreatedAt = date;
    }
}
