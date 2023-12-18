using Domain.Abstractions;

namespace Example.Domain.Entities;

public sealed class BankCard : CreationEntity<Guid>
{
    private BankCard()
        : base(Guid.Empty)
    {
        Number = Name = string.Empty;
    }

    public BankCard(string number, string name, DateOnly expiry)
        : this()
    {
        Number = number;
        Name = name;
        Expiry = expiry;
    }

    public string Number { get; private set; }

    public string Name { get; private set; }

    public DateOnly Expiry { get; private set; }
}
