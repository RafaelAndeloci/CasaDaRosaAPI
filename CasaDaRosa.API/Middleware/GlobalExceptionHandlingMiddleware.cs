using System.Net;
using System.Text.Json;
using CasaDaRosa.API.Contracts.Responses;
using CasaDaRosa.Domain.Exceptions;
using FluentValidation;

namespace CasaDaRosa.API.Middleware;

public sealed class GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception while processing request.");
            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;
        var response = exception switch
        {
            DomainValidationException domainValidationException => CreateDomainValidationError(domainValidationException, traceId),
            DomainException domainException => CreateDomainError(domainException, traceId),
            ValidationException validationException => CreateFluentValidationError(validationException, traceId),
            _ => CreateUnexpectedError(traceId)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)response.StatusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response.Body));
    }

    private static (HttpStatusCode StatusCode, ApiErrorResponse Body) CreateDomainValidationError(DomainValidationException exception, string traceId)
    {
        var errors = exception.Errors
            .Select(error => new ApiErrorDetail(exception.Code, error))
            .ToArray();

        return (
            HttpStatusCode.UnprocessableEntity,
            new ApiErrorResponse(false, exception.Code, exception.Message, errors, traceId));
    }

    private static (HttpStatusCode StatusCode, ApiErrorResponse Body) CreateDomainError(DomainException exception, string traceId)
    {
        return (
            HttpStatusCode.BadRequest,
            new ApiErrorResponse(false, exception.Code, exception.Message, [], traceId));
    }

    private static (HttpStatusCode StatusCode, ApiErrorResponse Body) CreateFluentValidationError(ValidationException exception, string traceId)
    {
        var errors = exception.Errors
            .Select(error => new ApiErrorDetail(error.PropertyName, error.ErrorMessage))
            .ToArray();

        return (
            HttpStatusCode.BadRequest,
            new ApiErrorResponse(false, "validation_error", "One or more validation errors occurred.", errors, traceId));
    }

    private static (HttpStatusCode StatusCode, ApiErrorResponse Body) CreateUnexpectedError(string traceId)
    {
        return (
            HttpStatusCode.InternalServerError,
            new ApiErrorResponse(false, "internal_server_error", "An unexpected error occurred.", [], traceId));
    }
}
