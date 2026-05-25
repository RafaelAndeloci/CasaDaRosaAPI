using CasaDaRosa.Application.Features.Users.Queries.GetMe;
using CasaDaRosa.Application.Features.Admin.Users.Commands.ActivateUser;
using CasaDaRosa.Application.Features.Admin.Users.Commands.DeactivateUser;
using CasaDaRosa.Application.Features.Admin.Users.Common;
using CasaDaRosa.Application.Features.Admin.Users.Queries.GetUserById;
using CasaDaRosa.Application.Features.Admin.Users.Queries.GetUsers;
using CasaDaRosa.Application.Common.Pagination;
using CasaDaRosa.Application.Common.Security;
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

    /// <summary>
    /// Retrieves users for administrative management with pagination and filters.
    /// </summary>
    /// <param name="query">Pagination and filtering parameters for the user listing.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>A paginated list of users.</returns>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AdminUserListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(query, cancellationToken);
        return OkResponse(response, "Users retrieved successfully.");
    }

    /// <summary>
    /// Retrieves a user for administrative management.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The user payload when found.</returns>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AdminUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new GetUserByIdQuery(id), cancellationToken);
        return OkResponse(response, "User retrieved successfully.");
    }

    /// <summary>
    /// Activates a user account.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpPatch("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new ActivateUserCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Deactivates a user account.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new DeactivateUserCommand(id), cancellationToken);
        return NoContent();
    }
}
