using CasaDaRosa.Application.Features.Carts.Commands.AddItemToCart;
using CasaDaRosa.Application.Features.Carts.Commands.ChangeCartItemQuantity;
using CasaDaRosa.Application.Features.Carts.Commands.RemoveCartItem;
using CasaDaRosa.Application.Features.Carts.Common;
using CasaDaRosa.Application.Features.Carts.Queries.GetMyCart;
using CasaDaRosa.API.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaDaRosa.API.Controllers;

[Authorize]
/// <summary>
/// Exposes authenticated user cart endpoints.
/// </summary>
public sealed class CartsController(ISender sender) : BaseController(sender)
{
    /// <summary>
    /// Retrieves the current cart of the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The current cart payload.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<CartResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new GetMyCartQuery(), cancellationToken);
        return OkResponse(response, "Cart retrieved successfully.");
    }

    /// <summary>
    /// Adds an item to the authenticated user's cart.
    /// </summary>
    /// <param name="command">Cart item payload.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The updated cart payload.</returns>
    [HttpPost("items")]
    [ProducesResponseType(typeof(ApiResponse<CartResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddItem([FromBody] AddItemToCartCommand command, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(command, cancellationToken);
        return OkResponse(response, "Cart item added successfully.");
    }

    /// <summary>
    /// Changes the quantity of an existing cart item.
    /// </summary>
    /// <param name="command">Cart item quantity update payload.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The updated cart payload.</returns>
    [HttpPut("items")]
    [ProducesResponseType(typeof(ApiResponse<CartResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangeQuantity([FromBody] ChangeCartItemQuantityCommand command, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(command, cancellationToken);
        return OkResponse(response, "Cart item quantity updated successfully.");
    }

    /// <summary>
    /// Removes an item from the authenticated user's cart.
    /// </summary>
    /// <param name="itemId">The cart item identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The updated cart payload.</returns>
    [HttpDelete("items/{itemId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CartResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveItem(Guid itemId, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new RemoveCartItemCommand(itemId), cancellationToken);
        return OkResponse(response, "Cart item removed successfully.");
    }
}
