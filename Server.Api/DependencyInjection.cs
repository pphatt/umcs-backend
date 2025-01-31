using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        services.AddSwaggerGen();

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
