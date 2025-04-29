using Microsoft.Extensions.DependencyInjection;

namespace ThinkWorld.Pii.Handlers;

public static class HandlersServiceCollectionExtensions
{
    public static IServiceCollection AddPiiHandlers(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(HandlersServiceCollectionExtensions)));
        return services;
    }
}