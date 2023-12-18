// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Abstractions;

public abstract class Entity<TId> :
    IEntity<TId>,
    IEquatable<Entity<TId>>
    where TId : struct
{
    protected Entity(TId id) =>
        Id = id;

    public TId Id { get; private init; }

    public bool Equals(Entity<TId>? obj)
    {
        if (ReferenceEquals(obj, null))
            return false;

        if (ReferenceEquals(obj, this))
            return true;

        return obj.GetType() == GetType() && Id.Equals(obj.Id);
    }

    public override bool Equals(object? obj) =>
        obj is Entity<TId> other && Equals(other);

    public override int GetHashCode() =>
        Id.GetHashCode();

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            return true;

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) =>
        !(left == right);

    public override string ToString() =>
        $"Entity {{ {GetType().Name} = {Id} }}";
}
