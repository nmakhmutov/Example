namespace Domain.Abstractions;

public abstract class CreationEntity<TKey> : Entity<TKey>,
    ICreated
    where TKey : struct
{
    protected CreationEntity(TKey id)
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