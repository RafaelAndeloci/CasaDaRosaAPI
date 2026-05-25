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
public sealed class CartsController(ISender sender) : BaseController(sender)
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<CartResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new GetMyCartQuery(), cancellationToken);
        return OkResponse(response, "Cart retrieved successfully.");
    }

    [HttpPost("items")]
    [ProducesResponseType(typeof(ApiResponse<CartResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AddItem([FromBody] AddItemToCartCommand command, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(command, cancellationToken);
        return OkResponse(response, "Cart item added successfully.");
    }

    [HttpPut("items")]
    [ProducesResponseType(typeof(ApiResponse<CartResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ChangeQuantity([FromBody] ChangeCartItemQuantityCommand command, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(command, cancellationToken);
        return OkResponse(response, "Cart item quantity updated successfully.");
    }

    [HttpDelete("items/{itemId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CartResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RemoveItem(Guid itemId, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new RemoveCartItemCommand(itemId), cancellationToken);
        return OkResponse(response, "Cart item removed successfully.");
    }
}
