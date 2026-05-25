using CasaDaRosa.Application.Features.Auth.Commands.Login;
using CasaDaRosa.Application.Features.Auth.Commands.ConfirmEmail;
using CasaDaRosa.Application.Features.Auth.Commands.Register;
using CasaDaRosa.Application.Features.Auth.Commands.ResendConfirmation;
using CasaDaRosa.API.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CasaDaRosa.API.Controllers;

public sealed class AuthController(ISender sender) : BaseController(sender)
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponse<RegisterResponse>(
            true,
            "User registered successfully.",
            response,
            HttpContext.TraceIdentifier));
    }

    [HttpPost("confirm-email")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        await Sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("resend-confirmation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationCommand command, CancellationToken cancellationToken)
    {
        await Sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(command, cancellationToken);
        return OkResponse(response, "Login processed successfully.");
    }
}
