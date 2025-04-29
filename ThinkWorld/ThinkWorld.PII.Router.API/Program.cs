using MediatR;
using ThinkWorld.Domain.Entities.Router;
using ThinkWorld.Domain.Events.Commands.Router;
using ThinkWorld.PII.Router.Handlers;
using ThinkWorld.Services;
using ThinkWorld.Services.DataContext;
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
builder.Services.AddRouterHandlers();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RouterDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
    
    // Seed initial data if needed
    await dbContext.SeedRegionsAsync();
}

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

app.MapGet("/api/router/user", async (string email, HttpContext httpContext, IMediator mediator) =>
    {
        var result = await mediator.Send(new GetRoutedUserCmd(email), httpContext.RequestAborted);

        if (result.HasErrors)
        {
            return Results.BadRequest(result.Errors);
        }
        
        if (result.Result == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(result.Result);
    })
    .WithName("GetRoutedUser")
    .WithOpenApi()
    .Produces<RoutedUser>()
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);
    // .RequireAuthorization();

app.MapGet("/api/router/regions", async (HttpContext httpContext, IMediator mediator) =>
    {
        var result = await mediator.Send(new GetPiiRegionsCmd(), httpContext.RequestAborted);

        if (result.HasErrors)
        {
            return Results.BadRequest(result.Errors);
        }

        return Results.Ok(result.Result);
    })
    .WithName("GetPiiRegions")
    .WithOpenApi()
    .Produces<RoutedUser>()
    .Produces(StatusCodes.Status400BadRequest);

app.Run();
