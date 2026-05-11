using CasaDaRosa.Domain.Abstractions;

namespace CasaDaRosa.Domain.Entities.Addresses;

public sealed class Address : AuditableEntity
{
    public Guid UserId { get; private set; }
    public Street Street { get; private set; }
    public AddressNumber Number { get; private set; }
    public string Neighborhood { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public AddressUf State { get; private set; }
    public ZipCode ZipCode { get; private set; }
    public string? Complement { get; private set; }
    public string? Reference { get; private set; }
    public bool IsDefault { get; private set; }
}
