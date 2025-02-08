using Server.Api;
using Server.Application;
using Server.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
{
    builder.Host.AddLogging();

    builder.Services
        .AddPresentation()
        .AddApplication()
        .AddInfrastructure(builder.Configuration);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("AdminAPI/swagger.json", "Admin API");
        c.DisplayOperationId();
        c.DisplayRequestDuration();
    });

    app.AddMigration();
}

// Add web application to the container.
{
    app.AddSerilog();

    app.AddAutoMapperValidation();
}

{
    app.UseExceptionHandler("/error");

    app.UseHttpsRedirection();

    //app.Use(async (context, next) =>
    //{
    //    var accessToken = context.Request.Query["access_token"];

    //    if (!string.IsNullOrEmpty(accessToken))
    //    {
    //        context.Request.Headers["Authorization"] = "Bearer " + accessToken;
    //    }

    //    await next.Invoke().ConfigureAwait(false);
    //});

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
