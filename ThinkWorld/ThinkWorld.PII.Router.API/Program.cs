using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ThinkWorld.Domain.Entities.Router;
using ThinkWorld.Domain.Events.Commands.Router;
using ThinkWorld.Events.Handlers.Handlers.Router;
using ThinkWorld.Services;
using ThinkWorld.Services.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
// builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddOptions<RouterDatabaseOptions>().ValidateOnStart();

var databaseOptions = builder.Configuration.GetSection(nameof(RouterDatabaseOptions)).Get<RouterDatabaseOptions>();

builder.Services.AddRouterCosmosContext(databaseOptions!);
builder.Services.AddCommonServices();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<AddOrUpdateRouterUserHandler>();
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(r => 
    {
        r.SwaggerEndpoint("/openapi/v1.json", "ThinkWorld PII Router API V1");
    });
}

app.UseHttpsRedirection();

app.MapPost("/api/router/user", async (AddOrUpdateRoutedUserCmd cmd, HttpContext httpContext, IMediator mediator) =>
    {
        var result = await mediator.Send(cmd, httpContext.RequestAborted);

        if (result.HasErrors)
        {
            return Results.BadRequest(result.Errors);
        }

        return Results.Ok(result.Result);
    })
    .WithName("AddOrUpdateUser")
    .WithOpenApi()
    .Produces<RoutedUser>()
    .Produces(StatusCodes.Status400BadRequest);
    // .RequireAuthorization();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}