namespace FinanceEdgeTrack.Infrastructure.Logging;

public class CustomerLoggerProviderConfig
{
    public LogLevel LogLevel { get; set; } = LogLevel.Warning;
    public int EventId { get; private set; } = 0;
}
