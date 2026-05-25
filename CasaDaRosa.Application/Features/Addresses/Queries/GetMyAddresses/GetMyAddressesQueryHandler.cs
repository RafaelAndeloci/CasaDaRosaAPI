using CasaDaRosa.Application.Abstractions.Contexts;
using CasaDaRosa.Application.Abstractions.Persistence;
using CasaDaRosa.Application.Common.Filters;
using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Exceptions;
using MediatR;

namespace CasaDaRosa.Application.Features.Addresses.Queries.GetMyAddresses;

public sealed class GetMyAddressesQueryHandler(IUserContext userContext, IAddressRepository addressRepository) : IRequestHandler<GetMyAddressesQuery, PagedResult<AddressListItemResponse>>
{
    public async Task<PagedResult<AddressListItemResponse>> Handle(GetMyAddressesQuery request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated || userContext.UserId is null)
        {
            throw new UnauthorizedApplicationException();
        }

        var addresses = await addressRepository.GetByUserIdAsync(userContext.UserId.Value, cancellationToken);

        var filteredAddresses = addresses
            .Where(address => TextFilterUtility.ContainsNormalized(address.City, request.City))
            .Where(address => TextFilterUtility.ContainsNormalized(address.State.Abbreviation.Code, request.State))
            .ToArray();

        var totalCount = filteredAddresses.Length;

        var pagedAddresses = filteredAddresses
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(address => new AddressListItemResponse(
                address.Id,
                address.Street.ToString(),
                address.Number.ToString(),
                address.Neighborhood,
                address.City,
                address.State.Abbreviation.Code,
                address.ZipCode.ToString(),
                address.Complement,
                address.Reference,
                address.IsDefault))
            .ToArray();

        return PagedResult<AddressListItemResponse>.Create(pagedAddresses, request.PageNumber, request.PageSize, totalCount);
    }
}
