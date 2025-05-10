using Azure.Identity;
using HealthChecks.CosmosDb;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ThinkWorld.Domain.Events.Commands.Router;
using ThinkWorld.Services.DataContext;
using ThinkWorld.Services.Options;

namespace ThinkWorld.Services;

public static class ServicesExtensions
{
    public static IServiceCollection AddGlobalCosmosContext(this IServiceCollection services, GlobalDatabaseOptions options)
    {
        var healthOptions = new AzureCosmosDbHealthCheckOptions()
        {
            DatabaseId = options.DatabaseName,
            ContainerIds = CosmosDbContext.GetTableNames()
        };
        var cosmosClientOptions = new CosmosClientOptions()
        {
            ApplicationRegion = options.ApplicationRegion,
        };
        if (options.UseManagedIdentity)
        {
            services.AddDbContext<CosmosDbContext>(o => o.UseCosmos(options.Endpoint, new DefaultAzureCredential(), 
                options.DatabaseName, x => x.Region(options.ApplicationRegion)));
            services.AddHealthChecks()
                .AddAzureCosmosDB(x => new CosmosClient(options.Endpoint, new DefaultAzureCredential(), cosmosClientOptions),
                    x => healthOptions, nameof(CosmosDbContext), tags: new[] { "CosmosDb", "all" });
        }
        else
        {
            if (string.IsNullOrEmpty(options.EndpointKey))
            {
                throw new ArgumentException($"{nameof(GlobalDatabaseOptions)}: EndpointKey cannot be null or empty when UseManagedIdentity is false.");
            }
            services.AddDbContext<CosmosDbContext>(o => o.UseCosmos(options.Endpoint, options.EndpointKey!, options.DatabaseName, x => x.Region(options.ApplicationRegion)));
            services.AddHealthChecks()
                .AddAzureCosmosDB(x => new CosmosClient(options.Endpoint, options.EndpointKey!, cosmosClientOptions),
                    x => healthOptions, nameof(CosmosDbContext), tags: new[] { "CosmosDb", "all" });
        }

        return services;
    }
    
    public static IServiceCollection AddRouterCosmosContext(this IServiceCollection services, RouterDatabaseOptions options)
    {
        var healthOptions = new AzureCosmosDbHealthCheckOptions()
        {
            DatabaseId = options.DatabaseName,
            ContainerIds = RouterDbContext.GetTableNames()
        };
        var cosmosClientOptions = new CosmosClientOptions()
        {
            ApplicationRegion = options.ApplicationRegion,
        };
        if (options.UseManagedIdentity)
        {
            services.AddDbContext<RouterDbContext>(o => o.UseCosmos(options.Endpoint, new DefaultAzureCredential(), options.DatabaseName,  x => x.Region(options.ApplicationRegion)));
            services.AddHealthChecks()
                .AddAzureCosmosDB(x => new CosmosClient(options.Endpoint, new DefaultAzureCredential(), cosmosClientOptions),
                    x => healthOptions, nameof(RouterDbContext), tags: new[] { "CosmosDb", "all" });
        }
        else
        {
            if (string.IsNullOrEmpty(options.EndpointKey))
            {
                throw new ArgumentException($"{nameof(RouterDatabaseOptions)}: EndpointKey cannot be null or empty when UseManagedIdentity is false.");
            }
            services.AddDbContext<RouterDbContext>(o => o.UseCosmos(options.Endpoint, options.EndpointKey!, options.DatabaseName,  x => x.Region(options.ApplicationRegion)));
            services.AddHealthChecks()
                .AddAzureCosmosDB(x => new CosmosClient(options.Endpoint, options.EndpointKey!, cosmosClientOptions),
                    x => healthOptions, nameof(RouterDbContext), tags: new[] { "CosmosDb", "all" });
        }

        return services;
    }
    
    public static IServiceCollection AddPiiCosmosContext(this IServiceCollection services, PiiDatabaseOptions options)
    {
        var healthOptions = new AzureCosmosDbHealthCheckOptions()
        {
            DatabaseId = options.DatabaseName,
            ContainerIds = UserDbContext.GetTableNames()
        };
        var cosmosClientOptions = new CosmosClientOptions()
        {
            ApplicationRegion = options.ApplicationRegion,
        };
        if (options.UseManagedIdentity)
        {
            services.AddDbContext<UserDbContext>(o => o.UseCosmos(options.Endpoint, new DefaultAzureCredential(), options.DatabaseName,  x => x.Region(options.ApplicationRegion)));
            services.AddHealthChecks()
                .AddAzureCosmosDB(x => new CosmosClient(options.Endpoint, new DefaultAzureCredential(), cosmosClientOptions),
                    x => healthOptions, nameof(UserDbContext), tags: new[] { "CosmosDb", "all" });
        }
        else
        {
            if (string.IsNullOrEmpty(options.EndpointKey))
            {
                throw new ArgumentException($"{nameof(PiiDatabaseOptions)}: EndpointKey cannot be null or empty when UseManagedIdentity is false.");
            }
            services.AddDbContext<UserDbContext>(o => o.UseCosmos(options.Endpoint, options.EndpointKey!, options.DatabaseName,  x => x.Region(options.ApplicationRegion)));
            services.AddHealthChecks()
                .AddAzureCosmosDB(x => new CosmosClient(options.Endpoint, options.EndpointKey!, cosmosClientOptions),
                    x => healthOptions, nameof(UserDbContext), tags: new[] { "CosmosDb", "all" });
            
        }

        return services;
    }
    
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        services.AddScoped<IUserIdGenerator, UserIdGenerator>();
        return services;
    }
}