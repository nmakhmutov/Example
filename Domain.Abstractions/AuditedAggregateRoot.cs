namespace Domain.Abstractions;

public abstract class AuditedAggregateRoot<TKey> : CreationAggregateRoot<TKey>
    where TKey : struct
{
    protected AuditedAggregateRoot(TKey id)
        : base(id) =>
        UpdatedAt = DateTime.MinValue;

    public DateTime UpdatedAt { get; protected set; }

    public override void SetAsUpdated(DateTime date)
    {
        base.SetAsUpdated(date);
        UpdatedAt = date;
    }
}
