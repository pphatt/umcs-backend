using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Server.Application.Common.Interfaces.Authentication;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Persistence.Authentication;
using Server.Application.Common.Interfaces.Services;
using Server.Domain.Entity.Identity;
using Server.Infrastructure.Authorization;
using Server.Infrastructure.Persistence;
using Server.Infrastructure.Persistence.Authentication;
using Server.Infrastructure.Services;
using System.Reflection;

namespace Server.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddScoped(typeof(IRepository<,>), typeof(RepositoryBase<,>));

        services.AddRepositories();

        services
            .AddDatabase(configuration)
            .AddDbIdentity()
            .AddAuthentication(configuration);

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            .EnableSensitiveDataLogging());

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Breakdown (Reflection in .net):
        // - typeof(TokenRepository): Retrieve the Type object for the TokenRepository class.
        // - .Assembly: Gets the assembly that contains the TokenRepository class.
        // - .GetTypes(): Will get all the classes, interfaces, struct, types, etc. out of the assembly where the Token Repository is defined.
        var concreteServices = typeof(TokenRepository).Assembly.GetTypes()
            // Filter out the interfaces and just take what interfaces have the same name as IRepository or inherited from IRepository interface.
            // The reason why filtering this out is because in the infrastructure layer have many implemented interfaces and services,
            // this can cause the GetTypes() to pick up all of that which we don't need, we just need the classes which inherited from the IRepository interface.
            .Where(x => x.GetInterfaces().Any(i => i.Name == typeof(IRepository<,>).Name)
                && !x.IsAbstract // exclude abstract class.
                && x.IsClass // include class.
                && !x.IsGenericType); // exclude generic type.

        foreach (var concreteService in concreteServices)
        {
            // get all interfaces which inherited by repository or services.
            // Ex: TokenRepository will return here { ITokenRepository, IRepository }, FacultyRepository will return here { IFacultyRepository, IRepository }.
            // if the classes have interface that have 3, 4 or n-level, here will return an array of interfaces.
            var allInterfaces = concreteService.GetInterfaces();

            // select all of the second level of the inherited (which here is IRepository).
            var inheritedInterface = allInterfaces.SelectMany(x => x.GetInterfaces());

            // exclude the second level and take the first level.
            var directInterface = allInterfaces.Except(inheritedInterface).FirstOrDefault();

            if (directInterface != null)
            {
                services.Add(new ServiceDescriptor(directInterface, concreteService, ServiceLifetime.Scoped));
            }
        }

        return services;
    }

    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SelectionName));

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }

    public static IServiceCollection AddDbIdentity(this IServiceCollection services)
    {
        services.AddIdentity<AppUser, AppRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            // Email settings.
            options.SignIn.RequireConfirmedEmail = true;

            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings.
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        });

        return services;
    }
}
