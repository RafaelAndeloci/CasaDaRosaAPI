using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Features.Categories.Queries.GetCategories;
using CasaDaRosa.API.Contracts.Responses;
using MediatR;
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
}
