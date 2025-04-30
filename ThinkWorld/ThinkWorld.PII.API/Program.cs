using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using ThinkWorld.Domain.Aggregates;
using ThinkWorld.Domain.Events.Commands.Comment;
using ThinkWorld.Domain.Events.Commands.Community;
using ThinkWorld.Domain.Events.Commands.Post;
using ThinkWorld.Domain.Events.Commands.User;
using ThinkWorld.Pii.Handlers;
using ThinkWorld.Services;
using ThinkWorld.Services.DataContext;
using ThinkWorld.Services.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddOptions<PiiDatabaseOptions>().ValidateOnStart();

var databaseOptions = builder.Configuration.GetSection(nameof(PiiDatabaseOptions)).Get<PiiDatabaseOptions>();

builder.Services.AddPiiCosmosContext(databaseOptions!);

builder.Services.AddCommonServices();
builder.Services.AddPiiHandlers();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(r =>
    {
        r.SwaggerEndpoint("/openapi/v1.json", "ThinkWorld PII API V1");
    });
}

app.UseHttpsRedirection();

app.MapPost("/api/comment", async (CreateCommentCmd cmd, HttpContext httpContext, IMediator mediator) =>
    {
        var result = await mediator.Send(cmd, httpContext.RequestAborted);

        if (result.HasErrors)
        {
            return Results.BadRequest(result.Errors);
        }

        return Results.Ok(result.Result);
    })
    .WithName("CreatePostComment")
    .WithOpenApi()
    .Produces<PostComment>()
    .Produces(StatusCodes.Status400BadRequest);

app.MapDelete("/api/comment/{commentId}", async ([FromRoute] Guid commentId, string email, HttpContext httpContext, IMediator mediator) =>
    {
        var result = await mediator.Send(new DeleteCommentCmd(commentId, email), httpContext.RequestAborted);

        if (result.HasErrors)
        {
            return Results.BadRequest(result.Errors);
        }

        return Results.NoContent();
    })
    .WithName("DeletePostComment")
    .WithOpenApi()
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status204NoContent);

app.MapGet("/api/post/{postId}/comments", async (Guid postId, HttpContext httpContext, IMediator mediator) =>
    {
        var result = await mediator.Send(new GetPostCommentsCmd(postId), httpContext.RequestAborted);

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
    .WithName("GetPostComments")
    .WithOpenApi()
    .Produces<List<PostComment>>()
    .Produces(StatusCodes.Status400BadRequest);


app.MapPost("/api/user", async (AddOrUpdateUserCmd cmd, HttpContext httpContext, IMediator mediator) =>
    {
        var result = await mediator.Send(cmd, httpContext.RequestAborted);

        if (result.HasErrors)
        {
            return Results.BadRequest(result.Errors);
        }

        return Results.Ok(result.Result);
    })
    .WithName("AddOrUpdateUserDetails")
    .WithOpenApi()
    .Produces<User>()
    .Produces(StatusCodes.Status400BadRequest);

app.MapDelete("/api/user/annonymise", async (string email, HttpContext httpContext, IMediator mediator) =>
    {
        var result = await mediator.Send(new AnnonymiseUserCmd(email), httpContext.RequestAborted);

        if (result.HasErrors)
        {
            return Results.BadRequest(result.Errors);
        }

        return Results.NoContent();
    })
    .WithName("AnnonymiseUser")
    .WithOpenApi()
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status204NoContent);

app.MapGet("/api/user", async (string email, HttpContext httpContext, IMediator mediator) =>
    {
        var result = await mediator.Send(new GetUserInfoCmd(email), httpContext.RequestAborted);

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
    .WithName("GetUserDetails")
    .WithOpenApi()
    .Produces<User>()
    .Produces(StatusCodes.Status400BadRequest);

app.MapPost("/api/post/vote", async (AddOrUpdatePostVoteCmd cmd, HttpContext httpContext, IMediator mediator) =>
    {
        var result = await mediator.Send(cmd, httpContext.RequestAborted);

        if (result.HasErrors)
        {
            return Results.BadRequest(result.Errors);
        }

        return Results.Ok(result.Result);
    })
    .WithName("AddOrUpdatePostVote")
    .WithOpenApi()
    .Produces<PostVote>()
    .Produces(StatusCodes.Status400BadRequest);

// .RequireAuthorization();

app.Run();
