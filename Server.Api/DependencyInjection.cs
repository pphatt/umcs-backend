using System.Reflection;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using Serilog;

using Server.Api.Authorization;
using Server.Api.Common.Errors;
using Server.Api.Common.Filters;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Entity.Identity;
using Server.Infrastructure;
using Server.Infrastructure.Persistence.Repositories;

namespace Server.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        //.AddJsonOptions(options =>
        //{
        //    // will not include the null value in the response.
        //    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        //});
        services.AddSignalR();

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
                    new List<string>()
                }
            });

            c.ParameterFilter<SwaggerNullableParameterFilter>();
        });

        // auto-mapper service.
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // add default error format.
        services.AddSingleton<ProblemDetailsFactory, ServerProblemDetailsFactory>();

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }

    public static IServiceCollection AddCors(this IServiceCollection services,
        ConfigurationManager configuration,
        string serverCorsPolicy)
    {
        services.AddCors(p => p.AddPolicy(serverCorsPolicy, builderCors =>
        {
            var origins = configuration["AllowedOrigins"];

            builderCors
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins(origins!);

            // add this for my signalr-test.html file
            builderCors
               .WithOrigins("http://127.0.0.1:5500")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
        }));

        return services;
    }
}

public static class MigrationManager
{
    public static async Task<WebApplication> AddMigration(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        var contributionRepository = scope.ServiceProvider.GetRequiredService<IContributionRepository>();

        // apply update-database command here.
        if ((await appDbContext.Database.GetPendingMigrationsAsync()).Any())
        {
            await appDbContext.Database.MigrateAsync();
        }

        if (await appDbContext.Database.CanConnectAsync())
        {
            await DataSeeder.SeedAsync(appDbContext, roleManager, contributionRepository);
        }

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
        //var scope = app.Services.CreateScope();
        //var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        //mapper.ConfigurationProvider.AssertConfigurationIsValid();

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
