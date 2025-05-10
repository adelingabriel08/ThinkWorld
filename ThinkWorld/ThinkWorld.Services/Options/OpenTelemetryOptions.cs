using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ThinkWorld.Services.Options;

public class OpenTelemetryOptions
{
    [Required] public string ServiceName { get; set; } = null!;
    [Required]
    public string ServiceNamespace { get; set; } = null!;
    [Required]
    public string Environment { get; set; } = null!;

    [Required] public string Region { get; set; } = null!;
    public string? ApplicationInsightsConnectionString { get; set; }
    public int SamplingProbability { get; set; } = 1;

}

public static class CustomMeters
{
    public const string MeterName = "ThinkWorld.Meter";
    public static readonly Meter ThinkWorldMeter = new Meter(MeterName);
}

public static class CustomTracing
{
    public const string ActivitySourceName = "ThinkWorld";
    public static ActivitySource ThinkWorldActivitySource { get; } 
        = new ActivitySource(ActivitySourceName);
}
