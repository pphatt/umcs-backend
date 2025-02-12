using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Server.Application.Common.Interfaces.Authentication;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Common.Interfaces.Services.Email;
using Server.Application.Common.Interfaces.Services.Media;
using Server.Domain.Entity.Identity;
using Server.Infrastructure.Authentication;
using Server.Infrastructure.Persistence;
using Server.Infrastructure.Persistence.Repositories;
using Server.Infrastructure.Services;
using Server.Infrastructure.Services.Email;
using Server.Infrastructure.Services.Media;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace Server.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddScoped(typeof(IRepository<,>), typeof(RepositoryBase<,>));

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IMediaService, MediaService>();

        services.AddHttpContextAccessor();

        services.AddRepositories();

        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<MediaSettings>(configuration.GetSection("MediaSettings"));
        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

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
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        JwtSettings jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SelectionName, jwtSettings);
        services.AddSingleton(Options.Create(jwtSettings));

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var result = string.Empty;

                        if (context.Response.HasStarted)
                        {
                            return Task.CompletedTask;
                        }

                        context.Response.ContentType = MediaTypeNames.Application.Json;

                        // is it token expired.
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                            result = JsonConvert.SerializeObject(new
                            {
                                message = "Token is expired.",
                                statusCode = (int)HttpStatusCode.Unauthorized,
                                status = "Unauthorized"
                            });
                        }
                        // or internal error.
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                            result = JsonConvert.SerializeObject(new { message = "Internal server error." });
                        }

                        return context.Response.WriteAsync(result);
                    },

                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        // Have to check this because when authorized access api failed,
                        // asp.net core web api will redirect to the 401 page and also we send the 401 message too.
                        // This will make the asp.net throw error.
                        if (context.Response.HasStarted)
                        {
                            return Task.CompletedTask;
                        }

                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Response.ContentType = MediaTypeNames.Application.Json;

                        var result = JsonConvert.SerializeObject(new { message = "You are not authorized." });

                        return context.Response.WriteAsync(result);
                    },

                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = MediaTypeNames.Application.Json;

                        var result = JsonConvert.SerializeObject(new { message = "You are not authorized to access these resources." });

                        return context.Response.WriteAsync(result);
                    }
                };
            });

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
