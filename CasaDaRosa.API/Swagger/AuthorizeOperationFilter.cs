using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CasaDaRosa.API.Swagger;

public sealed class AuthorizeOperationFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var apiDescription in context.ApiDescriptions)
        {
            var endpointMetadata = apiDescription.ActionDescriptor.EndpointMetadata;

            var hasAuthorize = endpointMetadata.OfType<AuthorizeAttribute>().Any();
            var hasAllowAnonymous = endpointMetadata.OfType<AllowAnonymousAttribute>().Any();

            if (!hasAuthorize || hasAllowAnonymous)
            {
                continue;
            }

            var operation = FindOperation(swaggerDoc, apiDescription);

            if (operation is null)
            {
                continue;
            }

            operation.Security =
            [
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", swaggerDoc, null)] = []
                }
            ];

            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
        }
    }

    private static OpenApiOperation? FindOperation(OpenApiDocument swaggerDoc, ApiDescription apiDescription)
    {
        var relativePath = apiDescription.RelativePath;

        if (string.IsNullOrWhiteSpace(relativePath) || string.IsNullOrWhiteSpace(apiDescription.HttpMethod))
        {
            return null;
        }

        var normalizedPath = $"/{relativePath.TrimStart('/')}";
        var pathEntry = swaggerDoc.Paths.FirstOrDefault(path => string.Equals(path.Key, normalizedPath, StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrWhiteSpace(pathEntry.Key))
        {
            return null;
        }

        var operationType = apiDescription.HttpMethod.ToUpperInvariant() switch
        {
            "GET" => HttpMethod.Get,
            "POST" => HttpMethod.Post,
            "PUT" => HttpMethod.Put,
            "DELETE" => HttpMethod.Delete,
            "PATCH" => HttpMethod.Patch,
            "HEAD" => HttpMethod.Head,
            "OPTIONS" => HttpMethod.Options,
            "TRACE" => HttpMethod.Trace,
            _ => null
        };

        if (operationType is null)
        {
            return null;
        }

        return pathEntry.Value.Operations.TryGetValue(operationType, out var operation)
            ? operation
            : null;
    }
}
