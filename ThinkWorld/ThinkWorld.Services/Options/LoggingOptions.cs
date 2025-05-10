namespace ThinkWorld.Services.Options;

public class LoggingOptions
{
    public ConsoleLoggingOptions Console { get; set; } = default!;
}

public class ConsoleLoggingOptions
{
    public string LoggingLevel { get; set; } = default!;
}
