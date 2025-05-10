using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using HealthChecks.CosmosDb;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ThinkWorld.Services.DataContext;
using ThinkWorld.Services.Options;

namespace ThinkWorld.Services;

public static class ServicesExtensions
{
    public static IServiceCollection AddGlobalCosmosContext(this IServiceCollection services,
        GlobalDatabaseOptions options)
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
                .AddAzureCosmosDB(
                    x => new CosmosClient(options.Endpoint, new DefaultAzureCredential(), cosmosClientOptions),
                    x => healthOptions, nameof(CosmosDbContext), tags: new[] { "CosmosDb", "all" });
        }
        else
        {
            if (string.IsNullOrEmpty(options.EndpointKey))
            {
                throw new ArgumentException(
                    $"{nameof(GlobalDatabaseOptions)}: EndpointKey cannot be null or empty when UseManagedIdentity is false.");
            }

            services.AddDbContext<CosmosDbContext>(o => o.UseCosmos(options.Endpoint, options.EndpointKey!,
                options.DatabaseName, x => x.Region(options.ApplicationRegion)));
            services.AddHealthChecks()
                .AddAzureCosmosDB(x => new CosmosClient(options.Endpoint, options.EndpointKey!, cosmosClientOptions),
                    x => healthOptions, nameof(CosmosDbContext), tags: new[] { "CosmosDb", "all" });
        }

        return services;
    }

    public static IServiceCollection AddRouterCosmosContext(this IServiceCollection services,
        RouterDatabaseOptions options)
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
            services.AddDbContext<RouterDbContext>(o => o.UseCosmos(options.Endpoint, new DefaultAzureCredential(),
                options.DatabaseName, x => x.Region(options.ApplicationRegion)));
            services.AddHealthChecks()
                .AddAzureCosmosDB(
                    x => new CosmosClient(options.Endpoint, new DefaultAzureCredential(), cosmosClientOptions),
                    x => healthOptions, nameof(RouterDbContext), tags: new[] { "CosmosDb", "all" });
        }
        else
        {
            if (string.IsNullOrEmpty(options.EndpointKey))
            {
                throw new ArgumentException(
                    $"{nameof(RouterDatabaseOptions)}: EndpointKey cannot be null or empty when UseManagedIdentity is false.");
            }

            services.AddDbContext<RouterDbContext>(o => o.UseCosmos(options.Endpoint, options.EndpointKey!,
                options.DatabaseName, x => x.Region(options.ApplicationRegion)));
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
            services.AddDbContext<UserDbContext>(o => o.UseCosmos(options.Endpoint, new DefaultAzureCredential(),
                options.DatabaseName, x => x.Region(options.ApplicationRegion)));
            services.AddHealthChecks()
                .AddAzureCosmosDB(
                    x => new CosmosClient(options.Endpoint, new DefaultAzureCredential(), cosmosClientOptions),
                    x => healthOptions, nameof(UserDbContext), tags: new[] { "CosmosDb", "all" });
        }
        else
        {
            if (string.IsNullOrEmpty(options.EndpointKey))
            {
                throw new ArgumentException(
                    $"{nameof(PiiDatabaseOptions)}: EndpointKey cannot be null or empty when UseManagedIdentity is false.");
            }

            services.AddDbContext<UserDbContext>(o => o.UseCosmos(options.Endpoint, options.EndpointKey!,
                options.DatabaseName, x => x.Region(options.ApplicationRegion)));
            services.AddHealthChecks()
                .AddAzureCosmosDB(x => new CosmosClient(options.Endpoint, options.EndpointKey!, cosmosClientOptions),
                    x => healthOptions, nameof(UserDbContext), tags: new[] { "CosmosDb", "all" });

        }

        return services;
    }

    public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserIdGenerator, UserIdGenerator>();
        services.Configure<OpenTelemetryOptions>(configuration.GetSection(nameof(OpenTelemetryOptions)));
        var openTelemetryOptions = new OpenTelemetryOptions();
        configuration.Bind(nameof(OpenTelemetryOptions), openTelemetryOptions);
        services.AddCustomOpenTelemetry(openTelemetryOptions);
        return services;
    }

    private static string[] _staticFilesExtensions =
    {
        ".css", ".js", ".gif", ".png", ".ico",
        ".map", ".woff", ".woff2", ".ttf", ".eot",
        ".svg", ".jpg", ".jpeg"
    };

    private static string[] _wwwrootfolders =
    {
        "/css/", "/fonts/", "/images/", "/js/", "/lib/"
    };

    public static IServiceCollection AddCustomOpenTelemetry(this IServiceCollection services, OpenTelemetryOptions options)
    {
        var serviceName = options.ServiceName ?? "unknown";
        var serviceNamespace = options.ServiceNamespace ?? "ThinkWorld";
        var environment = options.Environment ?? "development";

        AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

        services
            .AddOpenTelemetry()
            .ConfigureResource(builder =>
            {
                var fullServiceName = $"{serviceName.ToLower()} ({environment.ToLower()})";

                builder.AddService(fullServiceName, serviceNamespace, serviceInstanceId: Environment.MachineName)
                    .AddAttributes(new Dictionary<string, object>
                    {
                        { "deployment.environment", environment },
                        { "deployment.region", options.Region }
                    })
                    .AddTelemetrySdk();
            })
            .WithTracing(builder => builder.AddThinkWorldTracing(options))
            .WithMetrics(builder => builder.AddThinkWorldMetrics(options))
            .AddAzureMonitor(options);

        return services;
    }

    private static TracerProviderBuilder AddThinkWorldTracing(this TracerProviderBuilder builder, OpenTelemetryOptions options)
    {
        builder.AddSource(CustomTracing.ActivitySourceName);
        builder.AddSource("Azure.Cosmos.Operation");
        builder.AddSource("Azure.Cosmos.Operation");
        builder.AddHttpClientInstrumentation(options =>
        {
            options.RecordException = true;
            options.FilterHttpRequestMessage = (request) =>
            {
                return true;
            };
        });
        builder.AddAspNetCoreInstrumentation(options =>
        {
            options.RecordException = true;
            options.Filter = (httpContext) =>
            {
                return AspNetCoreInstrumentationFilter(httpContext);
            };
        });
        builder.AddEntityFrameworkCoreInstrumentation();

        if (options.SamplingProbability != 1)
        {
            builder.SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(options.SamplingProbability)));
        }
        
        builder.AddOtlpExporter();

        return builder;
    }

    private static bool AspNetCoreInstrumentationFilter(HttpContext httpContext)
    {
        if (httpContext.Request.Path.HasValue)
        {
            // if (httpContext.Request.Path.Value.Contains("health"))
            //     return false;

            if (_wwwrootfolders.Any(folder => httpContext.Request.Path.Value.StartsWith(folder)))
                return false;

            if (_staticFilesExtensions.Any(ext => httpContext.Request.Path.Value.EndsWith(ext)))
                return false;
        }

        return true;
    }

    private static OpenTelemetryBuilder AddAzureMonitor(this OpenTelemetryBuilder builder, OpenTelemetryOptions options)
    {
        if (!string.IsNullOrEmpty(options.ApplicationInsightsConnectionString))
            builder.UseAzureMonitor();

        return builder;
    }
    

    private static MeterProviderBuilder AddThinkWorldMetrics(this MeterProviderBuilder builder, OpenTelemetryOptions options)
    {
        builder.AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddMeter(CustomMeters.MeterName);

        return builder;
    }


}