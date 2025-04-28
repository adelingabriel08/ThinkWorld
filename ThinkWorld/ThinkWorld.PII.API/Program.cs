using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web.Resource;
using ThinkWorld.Events.Handlers.Handlers.Community;
using ThinkWorld.Events.Handlers.Handlers.Router;
using ThinkWorld.Events.Handlers.Handlers.User;
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
// builder.Services.AddMediatR(cfg =>
// {
//     cfg.RegisterServicesFromAssemblyContaining<CreateUserHandler>();
// });


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

var scopeRequiredByApi = app.Configuration["AzureAd:Scopes"] ?? "";
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", (HttpContext httpContext) =>
    {
        httpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();
    //.RequireAuthorization();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}