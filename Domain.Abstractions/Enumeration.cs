using System.Collections.Frozen;
using System.Reflection;
using System.Runtime.CompilerServices;

// ReSharper disable NonReadonlyMemberInGetHashCode
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Abstractions;

public abstract class Enumeration<TEnum> :
    IEquatable<Enumeration<TEnum>>,
    IComparable<Enumeration<TEnum>>
    where TEnum : Enumeration<TEnum>
{
    private static readonly Lazy<FrozenDictionary<int, TEnum>> Values =
        new(() => GetAll().ToFrozenDictionary(x => x.Value));

    private static readonly Lazy<FrozenDictionary<string, TEnum>> Names =
        new(() => GetAll().ToFrozenDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase));

    protected Enumeration(int value, string name)
    {
        Value = value;
        Name = name;
    }

    public int Value { get; private set; }

    public string Name { get; private set; }

    public static IReadOnlyCollection<TEnum> List =>
        Values.Value.Values;

    public int CompareTo(Enumeration<TEnum>? other)
    {
        if (ReferenceEquals(this, other))
            return 0;

        if (ReferenceEquals(null, other))
            return 1;

        return Value.CompareTo(other.Value);
    }

    public bool Equals(Enumeration<TEnum>? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        return other is not null && Value.Equals(other.Value);
    }

    private static IEnumerable<TEnum> GetAll() =>
        typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<TEnum>();

    public override string ToString() =>
        Name;

    public sealed override bool Equals(object? obj) =>
        obj is Enumeration<TEnum> other && Equals(other);

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return 0;

        if (ReferenceEquals(null, obj))
            return 1;

        return obj is Enumeration<TEnum> other
            ? CompareTo(other)
            : throw new ArgumentException($"Object must be of type {nameof(Enumeration<TEnum>)}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sealed override int GetHashCode() =>
        Value.GetHashCode();

    public static TEnum FromValue(int value)
    {
        if (Values.Value.TryGetValue(value, out var item))
            return item;

        throw new ArgumentException($"{typeof(TEnum)} doesn't contain value {value}", nameof(value));
    }

    public static TEnum FromName(string name)
    {
        if (Names.Value.TryGetValue(name, out var item))
            return item;

        throw new ArgumentException($"{typeof(TEnum)} doesn't contain name {name}", nameof(name));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Enumeration<TEnum>? left, Enumeration<TEnum>? right) =>
        left?.Equals(right) ?? right is null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Enumeration<TEnum>? left, Enumeration<TEnum>? right) =>
        !(left == right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Enumeration<TEnum>? left, Enumeration<TEnum>? right) =>
        left?.CompareTo(right) < 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Enumeration<TEnum>? left, Enumeration<TEnum>? right) =>
        left?.CompareTo(right) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Enumeration<TEnum>? left, Enumeration<TEnum>? right) =>
        left?.CompareTo(right) <= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Enumeration<TEnum>? left, Enumeration<TEnum>? right) =>
        left?.CompareTo(right) >= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Enumeration<TEnum>(int value) =>
        FromValue(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Enumeration<TEnum>(string name) =>
        FromName(name);
}
