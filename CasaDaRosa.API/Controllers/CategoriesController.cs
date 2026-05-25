using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Common.Security;
using CasaDaRosa.Application.Features.Admin.Categories.Commands.ActivateCategory;
using CasaDaRosa.Application.Features.Admin.Categories.Commands.CreateCategory;
using CasaDaRosa.Application.Features.Admin.Categories.Commands.DeactivateCategory;
using CasaDaRosa.Application.Features.Admin.Categories.Commands.UpdateCategory;
using CasaDaRosa.Application.Features.Admin.Categories.Common;
using CasaDaRosa.Application.Features.Admin.Categories.Queries.GetCategories;
using CasaDaRosa.Application.Features.Admin.Categories.Queries.GetCategoryById;
using CasaDaRosa.Application.Features.Categories.Queries.GetCategories;
using CasaDaRosa.API.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaDaRosa.API.Controllers;

/// <summary>
/// Exposes catalog category queries.
/// </summary>
public sealed class CategoriesController(ISender sender) : BaseController(sender)
{
    /// <summary>
    /// Retrieves the catalog categories with pagination and optional filters.
    /// </summary>
    /// <param name="query">Pagination and filtering parameters for the category listing.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>A paginated list of categories.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CategoryListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get([FromQuery] GetCategoriesQuery query, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(query, cancellationToken);
        return OkResponse(response, "Categories retrieved successfully.");
    }

    /// <summary>
    /// Retrieves categories for administrative management with pagination and optional filters.
    /// </summary>
    /// <param name="query">Pagination and filtering parameters for the category listing.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>A paginated list of categories for administrative management.</returns>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpGet("admin")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AdminCategoryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAdminCategories([FromQuery] GetAdminCategoriesQuery query, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(query, cancellationToken);
        return OkResponse(response, "Categories retrieved successfully.");
    }

    /// <summary>
    /// Retrieves a category for administrative management.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The category payload when found.</returns>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpGet("admin/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AdminCategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new GetCategoryByIdQuery(id), cancellationToken);
        return OkResponse(response, "Category retrieved successfully.");
    }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    /// <param name="command">Category creation payload.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The created category payload.</returns>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AdminCategoryResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(command, cancellationToken);
        return CreatedResponse(response, "Category created successfully.");
    }

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="command">Category update payload.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The updated category payload.</returns>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AdminCategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(command with { CategoryId = id }, cancellationToken);
        return OkResponse(response, "Category updated successfully.");
    }

    /// <summary>
    /// Activates a category.
    /// </summary>
    /// <param name="id">The category identifier.</param>
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
        await Sender.Send(new ActivateCategoryCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Deactivates a category.
    /// </summary>
    /// <param name="id">The category identifier.</param>
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
        await Sender.Send(new DeactivateCategoryCommand(id), cancellationToken);
        return NoContent();
    }
}
