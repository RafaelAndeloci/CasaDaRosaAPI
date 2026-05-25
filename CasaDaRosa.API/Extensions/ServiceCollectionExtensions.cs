using System.Reflection;
using CasaDaRosa.Application.DependencyInjection;
using CasaDaRosa.API.Swagger;
using CasaDaRosa.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using FluentValidation;
using Microsoft.OpenApi;

namespace CasaDaRosa.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddApplication();
        services.AddValidatorsFromAssembly(Assembly.Load("CasaDaRosa.Application"));
        services.AddInfrastructure(configuration);
        services.AddSwaggerDocumentation();

        return services;
    }

    private static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "Casa da Rosa API",
                Version = "v1",
                Description = "API para a loja de geleias e itens artesanais. Rotas protegidas exigem JWT Bearer no header Authorization."
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Cole apenas o token JWT. O Swagger adiciona automaticamente o prefixo Bearer no header Authorization."
            });

            options.DocumentFilter<AuthorizeOperationFilter>();

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}
