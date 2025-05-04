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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
// builder.Services.AddAuthorization();

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

app.MapPost("/api/community", async (AddOrUpdateCommunityCmd cmd, HttpContext httpContext, IMediator mediator) =>
    {
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
    .Produces(StatusCodes.Status400BadRequest);

app.MapPost("/api/post", async (AddOrUpdatePostCmd cmd, HttpContext httpContext, IMediator mediator) =>
    {
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
    .Produces(StatusCodes.Status400BadRequest);

app.MapDelete("/api/{communityId}/post", async ([FromRoute] Guid communityId, Guid postId, string email, HttpContext httpContext, IMediator mediator) =>
    {
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
    .Produces(StatusCodes.Status204NoContent);

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
    .Produces(StatusCodes.Status400BadRequest);
// .RequireAuthorization();

app.MapGet("/api/post", async (Guid? communityId, string email, HttpContext httpContext, IMediator mediator) =>
    {
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
    .Produces(StatusCodes.Status400BadRequest);
    
app.Run();