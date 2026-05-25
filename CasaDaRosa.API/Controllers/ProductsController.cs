using CasaDaRosa.Application.Features.Products.Queries.GetProducts;
using CasaDaRosa.Application.Features.Products.Queries.GetProductById;
using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.API.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CasaDaRosa.API.Controllers;

public sealed class ProductsController(ISender sender) : BaseController(sender)
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get([FromQuery] GetProductsQuery query, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(query, cancellationToken);
        return OkResponse(response, "Products retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new GetProductByIdQuery(id), cancellationToken);

        if (response is null)
        {
            return NotFound(new ApiErrorResponse(
                false,
                "products.not_found",
                "Product not found.",
                [],
                HttpContext.TraceIdentifier));
        }

        return OkResponse(response, "Product retrieved successfully.");
    }
}
