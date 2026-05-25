namespace CasaDaRosa.Application.Features.Addresses.Queries.GetMyAddresses;

public sealed record AddressListItemResponse(
    Guid Id,
    string Street,
    string Number,
    string Neighborhood,
    string City,
    string State,
    string ZipCode,
    string? Complement,
    string? Reference,
    bool IsDefault);
