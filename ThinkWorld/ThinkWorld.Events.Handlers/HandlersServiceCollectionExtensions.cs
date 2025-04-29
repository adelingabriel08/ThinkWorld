using Microsoft.Extensions.DependencyInjection;

namespace ThinkWorld.Events.Handlers;

public static class HandlersServiceCollectionExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(HandlersServiceCollectionExtensions)));
        return services;
    }
}