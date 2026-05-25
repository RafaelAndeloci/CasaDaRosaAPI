using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Features.Categories.Queries.GetCategories;
using CasaDaRosa.API.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CasaDaRosa.API.Controllers;

public sealed class CategoriesController(ISender sender) : BaseController(sender)
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CategoryListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get([FromQuery] GetCategoriesQuery query, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(query, cancellationToken);
        return OkResponse(response, "Categories retrieved successfully.");
    }
}
