using CasaDaRosa.Application.Common.Security;
using CasaDaRosa.API.Contracts.Responses;
using CasaDaRosa.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaDaRosa.API.Controllers;

/// <summary>
/// Exposes administrative endpoints to execute database seeding manually.
/// </summary>
[Route("api/admin/database-seed")]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
public sealed class DatabaseSeedController(DatabaseSeeder databaseSeeder) : ControllerBase
{
    /// <summary>
    /// Applies the configured seed data for users, categories and products.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Seed(CancellationToken cancellationToken)
    {
        await databaseSeeder.SeedAsync(cancellationToken);

        return Ok(new ApiResponse<object>(
            true,
            "Database seed applied successfully.",
            new { },
            HttpContext.TraceIdentifier));
    }
}
