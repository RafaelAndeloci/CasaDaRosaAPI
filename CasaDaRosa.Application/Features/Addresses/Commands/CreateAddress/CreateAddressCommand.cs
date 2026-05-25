using MediatR;

namespace CasaDaRosa.Application.Features.Addresses.Commands.CreateAddress;

public sealed record CreateAddressCommand(
    string Street,
    short Number,
    string Neighborhood,
    string City,
    string State,
    string ZipCode,
    string? Complement,
    string? Reference,
    bool IsDefault) : IRequest<CreateAddressResponse>;
