using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ThinkWorld.Services.DataContext;
using ThinkWorld.Services.Options;

namespace ThinkWorld.Services;

public static class ServicesExtensions
{
    public static IServiceCollection AddGlobalCosmosContext(this IServiceCollection services, GlobalDatabaseOptions options)
    {
        if (options.UseManagedIdentity)
        {
            services.AddDbContext<CosmosDbContext>(o => o.UseCosmos(options.Endpoint, new DefaultAzureCredential(), options.DatabaseName));
        }
        else
        {
            if (string.IsNullOrEmpty(options.EndpointKey))
            {
                throw new ArgumentException($"{nameof(GlobalDatabaseOptions)}: EndpointKey cannot be null or empty when UseManagedIdentity is false.");
            }
            services.AddDbContext<CosmosDbContext>(o => o.UseCosmos(options.Endpoint, options.EndpointKey!, options.DatabaseName));
        }

        return services;
    }
    
    public static IServiceCollection AddRouterCosmosContext(this IServiceCollection services, RouterDatabaseOptions options)
    {
        if (options.UseManagedIdentity)
        {
            services.AddDbContext<RouterDbContext>(o => o.UseCosmos(options.Endpoint, new DefaultAzureCredential(), options.DatabaseName));
        }
        else
        {
            if (string.IsNullOrEmpty(options.EndpointKey))
            {
                throw new ArgumentException($"{nameof(RouterDatabaseOptions)}: EndpointKey cannot be null or empty when UseManagedIdentity is false.");
            }
            services.AddDbContext<RouterDbContext>(o => o.UseCosmos(options.Endpoint, options.EndpointKey!, options.DatabaseName));
        }

        return services;
    }
    
    public static IServiceCollection AddPiiCosmosContext(this IServiceCollection services, PiiDatabaseOptions options)
    {
        if (options.UseManagedIdentity)
        {
            services.AddDbContext<UserDbContext>(o => o.UseCosmos(options.Endpoint, new DefaultAzureCredential(), options.DatabaseName));
        }
        else
        {
            if (string.IsNullOrEmpty(options.EndpointKey))
            {
                throw new ArgumentException($"{nameof(PiiDatabaseOptions)}: EndpointKey cannot be null or empty when UseManagedIdentity is false.");
            }
            services.AddDbContext<UserDbContext>(o => o.UseCosmos(options.Endpoint, options.EndpointKey!, options.DatabaseName));
        }

        return services;
    }
}