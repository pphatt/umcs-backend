using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Server.Domain.Entity.Identity;
using Server.Infrastructure;
using System.Reflection;

namespace Server.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();

            c.SwaggerDoc("AdminAPI", new OpenApiInfo 
            {
                Version = "v1",
                Title = "School CMS API",
                Description = "This API focuses on the core CMS functionality, handling campaign management, campaign rules, and campaign execution.",
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description = "To access this API, provide your access token."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme,
                        },
                        Scheme = "Bearer",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    []
                }
            });
        });

        // auto-mapper service.
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}

public static class MigrationManager
{
    public static WebApplication AddMigration(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

        // apply update-database command here.
        appDbContext.Database.Migrate();
        DataSeeder.SeedAsync(appDbContext, roleManager).GetAwaiter().GetResult();

        return app;
    }
}

public static class SerilogManager
{
    public static WebApplication AddSerilog(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        return app;
    }
}

public static class AutoMapperManager
{
    public static WebApplication AddAutoMapperValidation(this WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        mapper.ConfigurationProvider.AssertConfigurationIsValid();

        return app;
    }
}

public static class LoggingManager
{
    public static ConfigureHostBuilder AddLogging(this ConfigureHostBuilder host)
    {
        host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration)
        );

        return host;
    }
}
