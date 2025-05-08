using System.Configuration;
using System.Security.Claims;
using Azure.Identity;
using MediatR;
using ThinkWorld.Domain.Entities.Router;
using ThinkWorld.Domain.Events.Commands.Router;
using ThinkWorld.PII.Router.Handlers;
using ThinkWorld.Services;
using ThinkWorld.Services.DataContext;
using ThinkWorld.Services.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://dev-99631801.okta.com/oauth2/default";
        options.Audience = "api://default";
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                // Handle array-type scope claims from Okta
                var scopeClaim = context.Principal?.FindFirst("scp");
                if (scopeClaim != null && scopeClaim.Value.Contains("thinkworld.api"))
                {
                    // Already exists and contains our scope, we're good
                    return Task.CompletedTask;
                }

                // Look for scope claims in various formats that Okta might send
                var claims = context.Principal?.Claims.Where(c => 
                    (c.Type == "scp" || c.Type == "scope" || c.Type == "http://schemas.microsoft.com/identity/claims/scope") && 
                    c.Value.Contains("thinkworld.api")).ToList();
                
                if (claims != null && claims.Any())
                {
                    // We found our scope in one of the claim formats, we're good
                    return Task.CompletedTask;
                }

                // Debug-friendly message for token validation issues
                var tokenClaims = string.Join(", ", context.Principal?.Claims.Select(c => $"{c.Type}: {c.Value}") ?? Array.Empty<string>());
                context.Fail($"Token validation failed: Required scope 'thinkworld.api' not found. Token claims: {tokenClaims}");
                
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireThinkWorldApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        // Simplified scope check since we've already validated in the JwtBearerEvents
        policy.RequireAssertion(context => true);
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddOptions<RouterDatabaseOptions>().ValidateOnStart();

var databaseOptions = builder.Configuration.GetSection(nameof(RouterDatabaseOptions)).Get<RouterDatabaseOptions>();

builder.Services.AddRouterCosmosContext(databaseOptions!);
builder.Services.AddCommonServices();
builder.Services.AddRouterHandlers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
        });
});

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
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/router/user", async (AddOrUpdateRoutedUserCmd cmd, HttpContext httpContext, IMediator mediator) =>
    {
        var email = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(email))
        {
            return Results.BadRequest("Email claim is missing");
        }
        cmd = cmd with { Email = email };
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
    .Produces(StatusCodes.Status400BadRequest)
    .RequireAuthorization("RequireThinkWorldApiScope");

app.MapGet("/api/router/user", async (HttpContext httpContext, IMediator mediator) =>
    {
        var email = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(email))
        {
            return Results.BadRequest("Email claim is missing");
        }
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
    .Produces(StatusCodes.Status404NotFound)
    .RequireAuthorization("RequireThinkWorldApiScope");

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
    .Produces(StatusCodes.Status400BadRequest)
    .RequireAuthorization("RequireThinkWorldApiScope");

app.MapHealthChecks("/health");

app.Run();
