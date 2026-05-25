using MediatR;
using Microsoft.AspNetCore.Mvc;
using CasaDaRosa.API.Contracts.Responses;

namespace CasaDaRosa.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController(ISender sender) : ControllerBase
{
    protected ISender Sender { get; } = sender;

    protected IActionResult OkResponse<T>(T data, string message = "Request processed successfully.")
    {
        return Ok(new ApiResponse<T>(true, message, data, HttpContext.TraceIdentifier));
    }

    protected IActionResult CreatedResponse<T>(T data, string message = "Resource created successfully.")
    {
        return Created(string.Empty, new ApiResponse<T>(true, message, data, HttpContext.TraceIdentifier));
    }
}
