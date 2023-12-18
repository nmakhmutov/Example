namespace Domain.Abstractions;

public abstract class ValueObject : IEquatable<ValueObject>
{
    public bool Equals(ValueObject? obj)
    {
        if (ReferenceEquals(obj, null))
            return false;

        if (ReferenceEquals(obj, this))
            return true;

        if (obj.GetType() != GetType())
            return false;

        return GetEqualityComponents()
            .SequenceEqual(obj.GetEqualityComponents());
    }

    public override bool Equals(object? obj) =>
        obj is ValueObject other && Equals(other);

    public override int GetHashCode() =>
        GetEqualityComponents()
            .Select(x => x is null ? 0 : x.GetHashCode())
            .Aggregate((x, y) => x ^ y);

    protected abstract IEnumerable<object?> GetEqualityComponents();

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            return true;

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right) =>
        !(left == right);
}
