using CasaDaRosa.Application.Features.Products.Queries.GetProducts;
using CasaDaRosa.Application.Features.Products.Queries.GetProductById;
using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Common.Security;
using CasaDaRosa.Application.Features.Admin.Products.Commands.ActivateProduct;
using CasaDaRosa.Application.Features.Admin.Products.Commands.CreateProduct;
using CasaDaRosa.Application.Features.Admin.Products.Commands.DeactivateProduct;
using CasaDaRosa.Application.Features.Admin.Products.Commands.UpdateProduct;
using CasaDaRosa.Application.Features.Admin.Products.Common;
using CasaDaRosa.Application.Features.Admin.Products.Queries.GetProductById;
using CasaDaRosa.Application.Features.Admin.Products.Queries.GetProducts;
using CasaDaRosa.API.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaDaRosa.API.Controllers;

/// <summary>
/// Exposes catalog product queries.
/// </summary>
public sealed class ProductsController(ISender sender) : BaseController(sender)
{
    /// <summary>
    /// Retrieves the catalog products with pagination and optional filters.
    /// </summary>
    /// <param name="query">Pagination and filtering parameters for the product listing.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>A paginated list of products.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get([FromQuery] GetProductsQuery query, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(query, cancellationToken);
        return OkResponse(response, "Products retrieved successfully.");
    }

    /// <summary>
    /// Retrieves the details of a specific product.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The product details when found.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Retrieves products for administrative management with pagination and optional filters.
    /// </summary>
    /// <param name="query">Pagination and filtering parameters for the product listing.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>A paginated list of products for administrative management.</returns>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpGet("admin")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AdminProductResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAdminProducts([FromQuery] GetAdminProductsQuery query, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(query, cancellationToken);
        return OkResponse(response, "Products retrieved successfully.");
    }

    /// <summary>
    /// Retrieves a product for administrative management.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The product payload when found.</returns>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpGet("admin/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AdminProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAdminById(Guid id, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new GetAdminProductByIdQuery(id), cancellationToken);
        return OkResponse(response, "Product retrieved successfully.");
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="command">Product creation payload.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The created product payload.</returns>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AdminProductResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(command, cancellationToken);
        return CreatedResponse(response, "Product created successfully.");
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="command">Product update payload.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The updated product payload.</returns>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AdminProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(command with { ProductId = id }, cancellationToken);
        return OkResponse(response, "Product updated successfully.");
    }

    /// <summary>
    /// Activates a product.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpPatch("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new ActivateProductCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Deactivates a product.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new DeactivateProductCommand(id), cancellationToken);
        return NoContent();
    }
}
