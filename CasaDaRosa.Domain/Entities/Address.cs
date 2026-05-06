using CasaDaRosa.Domain.Abstractions;

namespace CasaDaRosa.Domain.Entities;

public class Address : AuditableEntity
{
    public Guid UserId { get; private set; }
    public string Street { get; private set; } = string.Empty;
    public string Number { get; private set; } = string.Empty;
    public string Neighborhood { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string ZipCode { get; private set; } = string.Empty;
    public string? Complement { get; private set; }
    public string? Reference { get; private set; }
    public bool IsDefault { get; private set; }

    private Address()
    {
    }

    public Address(Guid userId, string street, string number, string neighborhood, string city, string state, string zipCode, string? complement, string? reference, bool isDefault)
    {
        UserId = userId;
        Street = street;
        Number = number;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        ZipCode = zipCode;
        Complement = complement;
        Reference = reference;
        IsDefault = isDefault;
    }
}
