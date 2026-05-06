using FluentValidation;
using CasaDaRosa.Application.Abstractions;
using CasaDaRosa.Application.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CasaDaRosa.Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

        return services;
    }
}
