using Serilog;

namespace Server.Api;

public static class DependencyInjection
{
    public static WebApplication AddSerilog(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        return app;
    }

    public static ConfigureHostBuilder AddLogging(this ConfigureHostBuilder host)
    {
        host.UseSerilog((context, configuration) => 
            configuration.ReadFrom.Configuration(context.Configuration)
        );

        return host;
    }
}
