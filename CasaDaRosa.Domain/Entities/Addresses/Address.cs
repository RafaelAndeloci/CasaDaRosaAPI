using CasaDaRosa.Domain.Abstractions;

namespace CasaDaRosa.Domain.Entities.Addresses;

public sealed class Address : AuditableEntity
{
    public Guid UserId { get; private set; }
    public Street Street { get; private set; }
    public AddressNumber Number { get; private set; }
    public string Neighborhood { get; private set; }
    public string City { get; private set; }
    public AddressUf State { get; private set; }
    public ZipCode ZipCode { get; private set; }
    public string? Complement { get; private set; }
    public string? Reference { get; private set; }
    public bool IsDefault { get; private set; }

    private Address(
        Guid id,
        Guid userId, 
        Street street, 
        AddressNumber number, 
        string neighborhood, 
        string city,
        AddressUf state, 
        ZipCode zipCode, 
        string? complement, 
        string? reference, 
        bool isDefault) : base (id)
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

    public static Address Create(
        Guid userId,
        string street,
        short addressNumber,
        string neighborhood,
        string city,
        string abbreviatedUf,
        string zipCode,
        string? complement,
        string? reference,
        bool isDefault)
    {
        var streetValue = Street.Create(street);
        var addressNumberValue = AddressNumber.Create(addressNumber);

        return new Address(
            id: Guid.NewGuid(),
            userId: userId,
            street: streetValue,
            number: addressNumberValue,
            neighborhood: neighborhood,
            city: city,
            state: AddressUf.Create(abbreviatedUf),
            zipCode: ZipCode.Create(zipCode),
            complement: complement,
            reference: reference,
            isDefault: isDefault);
    }
}
