﻿using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;

using Quartz;

using Server.Application.Common.Interfaces.Authentication;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Common.Interfaces.Services.Cache;
using Server.Application.Common.Interfaces.Services.Email;
using Server.Application.Common.Interfaces.Services.Media;
using Server.Application.Common.Interfaces.Services.Report;
using Server.Domain.Entity.Identity;
using Server.Infrastructure.Authentication;
using Server.Infrastructure.Caching;
using Server.Infrastructure.Jobs.JobSetup;
using Server.Infrastructure.Persistence;
using Server.Infrastructure.Persistence.AppDbConnection;
using Server.Infrastructure.Persistence.Repositories;
using Server.Infrastructure.Services;
using Server.Infrastructure.Services.Cache;
using Server.Infrastructure.Services.Email;
using Server.Infrastructure.Services.Media;
using Server.Infrastructure.Services.Report;

using StackExchange.Redis;

namespace Server.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddScoped(typeof(IRepository<,>), typeof(RepositoryBase<,>));

        services.AddMemoryCache();
        services.AddScoped<AcademicYearRepository>();
        services.AddScoped<FacultyRepository>();

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetSection("RedisSettings")["RedisConnection"]!));
        services.AddStackExchangeRedisCache(redisOptions =>
        {
            redisOptions.Configuration = configuration.GetSection("RedisSettings")["RedisConnection"];
        });

        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IEmailService, EmailService>();
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

        services.AddScoped<IMediaService, MediaService>();
        services.Configure<MediaSettings>(configuration.GetSection("MediaSettings"));
        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

        services.AddScoped<ICacheService, CacheService>();
        services.Configure<CacheSettings>(configuration.GetSection("RedisSettings"));

        services.AddSingleton<IAppDbConnectionFactory, AppDbConnectionFactory>();
        services.AddScoped<IContributionReportService, ContributionReportService>();

        services.AddHttpContextAccessor();

        services.AddRepositories();

        // Serialize enum variable from number to string value.
        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services
            .AddDatabase(configuration)
            .AddDbIdentity()
            .AddAuthentication(configuration);

        services.AddQuartz(options =>
        {
            options.UseMicrosoftDependencyInjectionJobFactory();
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        services.ConfigureOptions<JobSetup>();

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
                && !x.IsGenericType) // exclude generic type.
            .OrderBy(x => x.Name.Contains("Cache") ? 1 : 0); // Non-Cache first (0), Cache last (1), to make sure mem-cache works correctly.

        foreach (var concreteService in concreteServices)
        {
            Console.WriteLine(concreteService.Name);
            // get all interfaces which inherited by repository or services.
            // Ex: TokenRepository will return here { ITokenRepository, IRepository }, FacultyRepository will return here { IFacultyRepository, IRepository }.
            // if the classes have interface that have 3, 4 or n-level, here will return an array of interfaces.
            var allInterfaces = concreteService.GetInterfaces();

            // select all of the second level of the inherited (which here is IRepository).
            // explain more:
            // - The allInterfaces will have { IRepository, IAcademicYearRepository }
            // - First loop, x === IRepository and IRepository will return null because IRepository is a concrete interface
            // will not return anything when IRepository.GetInterfaces()
            // - Second loop, x == IAcademicYearRepository and IAcademicYearRepository will return IRepository
            // because IAcademicYearRepository inherited from IRepository so IAcademicYearRepository.GetInterfaces() will return IRepository
            // and then the allInterface will except "IRepository" and allInterfaces primarily have { IRepository, IAcademicYearRepository } now just IAcademicYearRepository
            // so return "IAcademicYearRepository".
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

    //public static IServiceCollection AddMemoryCacheRepositories(this IServiceCollection services)
    //{
    //    var concreteServices = typeof(TokenRepository).Assembly.GetTypes()
    //        .Where(x => x.GetInterfaces().Any(i => i.Name == typeof(IRepository<,>).Name)
    //            && !x.IsAbstract
    //            && x.IsClass
    //            && !x.IsGenericType);

    //    foreach (var concreteService in concreteServices)
    //    {
    //        var allInterfaces = concreteService.GetInterfaces();

    //        var inheritedInterface = allInterfaces.SelectMany(x => x.GetInterfaces());

    //        var directInterface = allInterfaces.Except(inheritedInterface).FirstOrDefault();

    //        if (directInterface != null)
    //        {
    //            services.Add(new ServiceDescriptor(directInterface, concreteService, ServiceLifetime.Scoped));
    //        }
    //    }

    //    return services;
    //}

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
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },

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
                        // or internal error (or this can be happened when access token is incorrect).
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

        services.Configure<DataProtectionTokenProviderOptions>(options =>
            // settings for all 4 tokens in TokenProviders not just Reset Password Token.
            options.TokenLifespan = TimeSpan.FromMinutes(30)
        );

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
