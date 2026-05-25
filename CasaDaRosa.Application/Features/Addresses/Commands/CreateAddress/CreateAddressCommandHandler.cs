using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Exceptions;
using CasaDaRosa.Domain.Entities.Addresses;
using MediatR;

namespace CasaDaRosa.Application.Features.Addresses.Commands.CreateAddress;

public sealed class CreateAddressCommandHandler(
    IUserContext userContext,
    IAddressRepository addressRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateAddressCommand, CreateAddressResponse>
{
    public async Task<CreateAddressResponse> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated || userContext.UserId is null)
        {
            throw new UnauthorizedApplicationException();
        }

        var address = Address.Create(
            userContext.UserId.Value,
            request.Street,
            request.Number,
            request.Neighborhood,
            request.City,
            request.State,
            request.ZipCode,
            request.Complement,
            request.Reference,
            request.IsDefault);

        await addressRepository.AddAsync(address, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateAddressResponse(address.Id);
    }
}
