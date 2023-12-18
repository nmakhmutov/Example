namespace Domain.Abstractions;

public abstract class AuditedEntity<TId> : CreationEntity<TId>
    where TId : struct
{
    protected AuditedEntity(TId id)
        : base(id) =>
        UpdatedAt = DateTime.MinValue;

    public DateTime UpdatedAt { get; private set; }

    public override void SetAsUpdated(DateTime date)
    {
        base.SetAsUpdated(date);
        UpdatedAt = date;
    }
}
