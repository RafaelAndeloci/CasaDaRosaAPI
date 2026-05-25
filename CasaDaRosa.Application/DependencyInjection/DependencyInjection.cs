using CasaDaRosa.Application.Abstractions;
using CasaDaRosa.Application.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using CasaDaRosa.Application.Behaviors;
using FluentValidation;

namespace CasaDaRosa.Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

        return services;
    }
}
