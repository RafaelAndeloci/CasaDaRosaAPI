using CasaDaRosa.Application.Common.Pagination;
using MediatR;

namespace CasaDaRosa.Application.Features.Addresses.Queries.GetMyAddresses;

public sealed record GetMyAddressesQuery(
    string? City = null,
    string? State = null,
    int PageNumber = 1,
    int PageSize = 10) : PagedQuery(PageNumber, PageSize), IRequest<PagedResult<AddressListItemResponse>>;
