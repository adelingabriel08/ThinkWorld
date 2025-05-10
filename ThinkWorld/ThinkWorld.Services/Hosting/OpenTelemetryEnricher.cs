using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace ThinkWorld.Services.Hosting;

public class OpenTelemetryEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var activity = Activity.Current;
        
        if (activity == null)
        {
            return;
        }
        
        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("trace.id", activity.TraceId));
        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("span.id", activity.SpanId));
        
        TryAddLogToEvent(activity, logEvent, propertyFactory, "user.id");
    }
    
    private static void TryAddLogToEvent(Activity activity, LogEvent logEvent, ILogEventPropertyFactory propertyFactory, string key)
    {
        var value = activity.GetTagItem(key);
        
        if (value != null)
        {
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(key, value));
        }
    }
}