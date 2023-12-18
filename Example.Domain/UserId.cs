namespace Example.Domain;

public readonly struct UserId :
    IEquatable<UserId>,
    IComparable<UserId>
{
    private static readonly UserId Default = new(0);
    private readonly long _value;

    public UserId(long userId) =>
        _value = userId;

    public static ref readonly UserId Empty =>
        ref Default;

    public int CompareTo(UserId other) =>
        _value.CompareTo(other._value);

    public int CompareTo(object? obj) =>
        obj switch
        {
            null => 1,
            UserId id => CompareTo(id),
            _ => throw new ArgumentException($"Object must be of type {nameof(UserId)}")
        };

    public bool Equals(UserId other) =>
        _value == other._value;

    public override bool Equals(object? obj) =>
        obj is UserId other && Equals(other);

    public override int GetHashCode() =>
        _value.GetHashCode();

    public override string ToString() =>
        _value.ToString();

    public static bool operator ==(UserId a, UserId b) =>
        a.CompareTo(b) == 0;

    public static bool operator !=(UserId a, UserId b) =>
        !(a == b);

    public static bool operator >(UserId a, UserId b) =>
        a._value > b._value;

    public static bool operator <(UserId a, UserId b) =>
        a._value < b._value;

    public static bool operator <=(UserId a, UserId b) =>
        a._value <= b._value;

    public static bool operator >=(UserId a, UserId b) =>
        a._value >= b._value;

    public static implicit operator long(UserId id) =>
        id._value;

    public static implicit operator UserId(long id) =>
        new(id);
}
