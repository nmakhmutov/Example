using System.Net.Mail;
using Domain.Abstractions;
using Example.Domain.DomainEvents;
using Example.Domain.Exceptions;
using Example.Domain.ValueObjects;

namespace Example.Domain.Entities;

public sealed class User : AuditedAggregateRoot<UserId>
{
    private readonly HashSet<BankCard> _cards;

    private User()
        : base(UserId.Empty)
    {
        Email = string.Empty;
        Name = default!;
        Address = default!;

        _cards = [];
    }

    private User(string email, Name name, Address address)
        : this()
    {
        Name = name;
        Address = address;
        Email = email;
    }

    public string Email { get; private set; }

    public Name Name { get; private set; }

    public Address Address { get; private set; }

    public IReadOnlyCollection<BankCard> Cards =>
        _cards.ToArray();

    public static User Create(string email, Name name, Address address)
    {
        if (!MailAddress.TryCreate(email, out _))
            throw new UserException($"Email address '{email}' is invalid");

        var user = new User(email, name, address);
        user.SetAsCreated();

        return user;
    }

    public void Update(Name name, Address address)
    {
        Name = name;
        Address = address;

        AddDomainEvent(new UserUpdatedDomainEvent(this));
    }

    public void AddCard(string number, string name, DateOnly expiry)
    {
        var card = new BankCard(number, name, expiry);
        _cards.Add(card);

        AddDomainEvent(new BankCardAddedDomainEvent(this, card));
    }

    private void SetAsCreated() =>
        AddDomainEvent(new UserCreatedDomainEvent(this));
}
