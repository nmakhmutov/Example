using Domain.Abstractions;

namespace Example.Domain.ValueObjects;

public sealed class Address : ValueObject
{
    public Address(string countryCode, string city, string street, string building)
    {
        CountryCode = countryCode;
        City = city;
        Street = street;
        Building = building;
    }

    public string CountryCode { get; }

    public string City { get; }

    public string Street { get; }

    public string Building { get; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CountryCode;
        yield return City;
        yield return Street;
        yield return Building;
    }
}
