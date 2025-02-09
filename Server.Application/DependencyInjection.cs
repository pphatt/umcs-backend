using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Server.Application.Common.Behaviors;
using System.Reflection;

namespace Server.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = Assembly.GetExecutingAssembly();

        // previous version of "MediatR.Extensions.Microsoft.DependencyInjection" is deprecated and not needed from v12 of MediatR.
        // old school way (previous of the v12 need to install "MediatR.Extensions.Microsoft.DependencyInjection" package):
        // services.AddMediatR(typeof(DependencyInjection).Assembly); -> MediatR v11.1.0
        // https://stackoverflow.com/a/72263414
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        services.AddValidatorsFromAssembly(applicationAssembly)
            .AddFluentValidationAutoValidation();

        return services;
    }
}
