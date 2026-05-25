using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Features.Addresses.Commands.CreateAddress;
using CasaDaRosa.Application.Features.Addresses.Queries.GetMyAddresses;
using CasaDaRosa.API.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaDaRosa.API.Controllers;

[Authorize]
public sealed class AddressesController(ISender sender) : BaseController(sender)
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AddressListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get([FromQuery] GetMyAddressesQuery query, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(query, cancellationToken);
        return OkResponse(response, "Addresses retrieved successfully.");
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreateAddressResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateAddressCommand command, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(command, cancellationToken);
        return OkResponse(response, "Address created successfully.");
    }
}
