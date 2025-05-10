using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;
using ThinkWorld.Services.Options;

namespace ThinkWorld.Services.Hosting;

public static class LoggingExtensions
{
    public static ILoggingBuilder AddCustomSerilog(this ILoggingBuilder builder, IServiceCollection services,
        IConfiguration configuration)
    {
        var loggerConfiguration = new LoggerConfiguration();
        loggerConfiguration.AddCustomSerilog(services, configuration);
        builder.AddSerilog(loggerConfiguration.CreateLogger());
        return builder;
    }

    public static LoggerConfiguration AddCustomSerilog(this LoggerConfiguration loggerConfiguration,
       IServiceCollection services, IConfiguration configuration)
    {
        var otlpOptions = new OpenTelemetryOptions();
        configuration.GetSection("OpenTelemetryOptions").Bind(otlpOptions);
        var loggingOptions = new LoggingOptions();
        configuration.GetSection("LoggingOptions").Bind(loggingOptions);

        var serviceName = otlpOptions.ServiceName ?? "unknown";
        var serviceNamespace = otlpOptions.ServiceName ?? "ThinkWorld";
        var environment = otlpOptions.Environment ?? "development";

        var fullServiceName = $"{serviceName.ToLower()} ({environment.ToLower()})";
        var telemetryClient = services.ConfigureApplicationInsights(configuration);

        loggerConfiguration
            .ReadFrom.Configuration(configuration)
            .ConfigureApplicationInsights(telemetryClient, TelemetryConverter.Traces)
            .ConfigureConsole(loggingOptions.Console)
            .Enrich.FromLogContext()
            .Enrich.With<OpenTelemetryEnricher>()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("service.name", fullServiceName)
            .Enrich.WithProperty("service.namespace", serviceNamespace)
            .Enrich.WithProperty("service.instance.id", Environment.MachineName)
            .Enrich.WithProperty("deployment.environment", environment);

        return loggerConfiguration;
    }

    public static TelemetryClient ConfigureApplicationInsights(this IServiceCollection services,
        IConfiguration configuration)
    {
        var otlpOptions = new OpenTelemetryOptions();
        configuration.GetSection("OpenTelemetryOptions").Bind(otlpOptions);

        var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
        if (!string.IsNullOrEmpty(otlpOptions.ApplicationInsightsConnectionString))
        {
            telemetryConfiguration.ConnectionString = otlpOptions.ApplicationInsightsConnectionString;
        }
        else
        {
            telemetryConfiguration.DisableTelemetry = true;
        }

        var telemetryClient = new TelemetryClient(telemetryConfiguration);
        services.AddSingleton(telemetryClient);

        return telemetryClient;
    }

    private static LoggerConfiguration ConfigureApplicationInsights(this LoggerConfiguration loggerConfiguration,
        TelemetryClient telemetryClient, ITelemetryConverter telemetryConverter)
    {
        loggerConfiguration
            .WriteTo
            .ApplicationInsights(telemetryClient, telemetryConverter);

        return loggerConfiguration;
    }


    private static LoggerConfiguration ConfigureConsole(this LoggerConfiguration loggerConfiguration,
        ConsoleLoggingOptions consoleOptions)
    {
        if (!string.IsNullOrEmpty(consoleOptions?.LoggingLevel))
        {
            if (!Enum.TryParse<LogEventLevel>(consoleOptions.LoggingLevel, true, out var loggingLevel))
                throw new InvalidOperationException("Invalid console logging level.");

            loggerConfiguration
                .WriteTo
                .Console(
                    restrictedToMinimumLevel: loggingLevel,
                    outputTemplate: "[{Level:u3}] {SourceContext}{NewLine}      {Message:lj}{NewLine}{Exception}");
        }

        return loggerConfiguration;
    }
}
