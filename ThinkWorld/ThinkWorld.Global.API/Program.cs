using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using ThinkWorld.Domain.Aggregates;
using ThinkWorld.Domain.Events.Commands.Community;
using ThinkWorld.Domain.Events.Commands.Post;
using ThinkWorld.Events.Handlers;
using ThinkWorld.Services;
using ThinkWorld.Services.DataContext;
using ThinkWorld.Services.Options;

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

builder.Services.AddOptions<GlobalDatabaseOptions>().ValidateDataAnnotations();

var globalDatabaseOptions = builder.Configuration.GetSection(nameof(GlobalDatabaseOptions)).Get<GlobalDatabaseOptions>();

builder.Services.AddGlobalCosmosContext(globalDatabaseOptions!);
builder.Services.AddCommonServices();
builder.Services.AddHandlers();
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
    var dbContext = scope.ServiceProvider.GetRequiredService<CosmosDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(r =>
    {
        r.SwaggerEndpoint("/openapi/v1.json", "ThinkWorld Global API V1");
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/community", async (AddOrUpdateCommunityCmd cmd, HttpContext httpContext, IMediator mediator) =>
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
    .WithName("AddOrUpdateCommunity")
    .WithOpenApi()
    .Produces<Community>()
    .Produces(StatusCodes.Status400BadRequest)
    .RequireAuthorization("RequireThinkWorldApiScope");

app.MapPost("/api/post", async (AddOrUpdatePostCmd cmd, HttpContext httpContext, IMediator mediator) =>
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
    .WithName("AddOrUpdatePost")
    .WithOpenApi()
    .Produces<CommunityPost>()
    .Produces(StatusCodes.Status400BadRequest)
    .RequireAuthorization("RequireThinkWorldApiScope");

app.MapDelete("/api/{communityId}/post", async ([FromRoute] Guid communityId, Guid postId, HttpContext httpContext, IMediator mediator) =>
    {
        var email = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(email))
        {
            return Results.BadRequest("Email claim is missing");
        }
        var result = await mediator.Send(new DeletePostCmd(postId, communityId, email), httpContext.RequestAborted);

        if (result.HasErrors)
        {
            return Results.BadRequest(result.Errors);
        }

        return Results.NoContent();
    })
    .WithName("DeletePost")
    .WithOpenApi()
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status204NoContent)
    .RequireAuthorization("RequireThinkWorldApiScope");

app.MapGet("/api/community", async (HttpContext httpContext, IMediator mediator) =>
    {
        var result = await mediator.Send(new GetCommunitiesCmd(), httpContext.RequestAborted);

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
    .WithName("GetCommunities")
    .WithOpenApi()
    .Produces<List<Community>>()
    .Produces(StatusCodes.Status400BadRequest)
    .RequireAuthorization("RequireThinkWorldApiScope");

app.MapGet("/api/post", async (Guid? communityId, HttpContext httpContext, IMediator mediator) =>
    {
        var email = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(email))
        {
            return Results.BadRequest("Email claim is missing");
        }
        var result = await mediator.Send(new GetPostsCmd(communityId, email), httpContext.RequestAborted);

        if (result.HasErrors)
        {
            return Results.BadRequest(result.Errors);
        }

        return Results.Ok(result.Result);
    })
    .WithName("GetPosts")
    .WithOpenApi()
    .Produces<List<CommunityPost>>()
    .Produces(StatusCodes.Status400BadRequest)
    .RequireAuthorization("RequireThinkWorldApiScope");

app.MapHealthChecks("/health");

app.Run();