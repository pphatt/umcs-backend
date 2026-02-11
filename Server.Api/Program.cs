using Server.Api;
using Server.Application;
using Server.Application.Hubs.Notifications;
using Server.Application.Hubs.PrivateChats;
using Server.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var serverCorsPolicy = "ServerCorsPolicy";

// Add services to the container.
{
    builder.Host.AddLogging();

    builder.Services
        .AddCors(builder.Configuration, serverCorsPolicy)
        .AddPresentation()
        .AddApplication()
        .AddInfrastructure(builder.Configuration);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("AdminAPI/swagger.json", "Admin API");
        c.DisplayOperationId();
        c.DisplayRequestDuration();
    });

    await app.AddMigration();
}

// Add web application to the container.
{
    app.AddSerilog();

    app.AddAutoMapperValidation();
}

{
    app.UseStaticFiles();

    app.UseCors(serverCorsPolicy);

    app.UseExceptionHandler("/error");

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.MapHub<NotificationHub>("/hubs/notifications");
    app.MapHub<PrivateChatHub>("/hubs/chat");

    app.Run();
}
