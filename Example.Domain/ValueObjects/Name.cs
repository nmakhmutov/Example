using Domain.Abstractions;
using Example.Domain.Exceptions;

namespace Example.Domain.ValueObjects;

public sealed class Name : ValueObject
{
    public const int MinFirstNameLen = 3;
    public const int MaxFirstNameLen = 256;

    public const int MinLastNameLen = 3;
    public const int MaxLastNameLen = 256;
    private Name(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; }

    public string LastName { get; }

    public static Name Create(string firstName, string lastName)
    {
        if (firstName.Length is < MinFirstNameLen or > MaxFirstNameLen)
            throw new UserException("First name cannot be less than 3 chars");

        if (lastName.Length is < MinLastNameLen or > MaxLastNameLen)
            throw new UserException("Last name cannot be less than 3 chars");

        return new Name(firstName, lastName);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}
