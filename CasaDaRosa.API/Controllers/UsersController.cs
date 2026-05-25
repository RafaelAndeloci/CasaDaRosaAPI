using CasaDaRosa.Application.Features.Users.Queries.GetMe;
using CasaDaRosa.API.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaDaRosa.API.Controllers;

[Authorize]
public sealed class UsersController(ISender sender) : BaseController(sender)
{
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new GetMeQuery(), cancellationToken);

        if (response is null)
        {
            return NotFound(new ApiErrorResponse(
                false,
                "users.me.not_found",
                "Authenticated user was not found.",
                [],
                HttpContext.TraceIdentifier));
        }

        return OkResponse(response, "User profile retrieved successfully.");
    }
}
