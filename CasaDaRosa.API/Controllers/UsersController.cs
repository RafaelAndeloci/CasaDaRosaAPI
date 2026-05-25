using CasaDaRosa.Application.Features.Users.Queries.GetMe;
using CasaDaRosa.API.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaDaRosa.API.Controllers;

[Authorize]
/// <summary>
/// Exposes authenticated user profile endpoints.
/// </summary>
public sealed class UsersController(ISender sender) : BaseController(sender)
{
    /// <summary>
    /// Retrieves the profile of the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The current user profile.</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
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
