using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Features.Addresses.Commands.CreateAddress;
using CasaDaRosa.Application.Features.Addresses.Queries.GetMyAddresses;
using CasaDaRosa.API.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaDaRosa.API.Controllers;

[Authorize]
/// <summary>
/// Exposes authenticated user address endpoints.
/// </summary>
public sealed class AddressesController(ISender sender) : BaseController(sender)
{
    /// <summary>
    /// Retrieves the addresses of the authenticated user with pagination.
    /// </summary>
    /// <param name="query">Pagination parameters for the address listing.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>A paginated list of addresses.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AddressListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get([FromQuery] GetMyAddressesQuery query, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(query, cancellationToken);
        return OkResponse(response, "Addresses retrieved successfully.");
    }

    /// <summary>
    /// Creates a new address for the authenticated user.
    /// </summary>
    /// <param name="command">Address creation payload.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The created address payload.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreateAddressResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateAddressCommand command, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(command, cancellationToken);
        return OkResponse(response, "Address created successfully.");
    }
}
