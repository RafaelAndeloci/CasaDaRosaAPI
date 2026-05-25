using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Features.Orders.Commands.CheckoutOrder;
using CasaDaRosa.Application.Features.Orders.Common;
using CasaDaRosa.Application.Features.Orders.Queries.GetMyOrders;
using CasaDaRosa.Application.Features.Orders.Queries.GetOrderById;
using CasaDaRosa.API.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaDaRosa.API.Controllers;

[Authorize]
/// <summary>
/// Exposes authenticated user order endpoints.
/// </summary>
public sealed class OrdersController(ISender sender) : BaseController(sender)
{
    /// <summary>
    /// Performs the checkout of the authenticated user's current cart.
    /// </summary>
    /// <param name="command">Checkout payload.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The created order payload.</returns>
    [HttpPost("checkout")]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Checkout([FromBody] CheckoutOrderCommand command, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(command, cancellationToken);
        return OkResponse(response, "Order checkout completed successfully.");
    }

    /// <summary>
    /// Retrieves the authenticated user's orders with pagination and filters.
    /// </summary>
    /// <param name="query">Pagination and filtering parameters for the order listing.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>A paginated list of orders.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OrderResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get([FromQuery] GetMyOrdersQuery query, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(query, cancellationToken);
        return OkResponse(response, "Orders retrieved successfully.");
    }

    /// <summary>
    /// Retrieves a specific order from the authenticated user.
    /// </summary>
    /// <param name="id">The order identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The order payload when found.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new GetOrderByIdQuery(id), cancellationToken);
        return OkResponse(response, "Order retrieved successfully.");
    }
}
