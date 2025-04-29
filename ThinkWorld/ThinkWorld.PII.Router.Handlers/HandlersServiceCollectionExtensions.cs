using Microsoft.Extensions.DependencyInjection;

namespace ThinkWorld.PII.Router.Handlers;

public static class HandlersServiceCollectionExtensions
{
    public static IServiceCollection AddRouterHandlers(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(HandlersServiceCollectionExtensions)));
        return services;
    }
}