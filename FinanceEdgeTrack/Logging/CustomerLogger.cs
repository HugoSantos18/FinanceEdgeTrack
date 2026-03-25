
namespace FinanceEdgeTrack.Logging;

public class CustomerLogger : ILogger
{
    private readonly CustomerLoggerProviderConfig _config;
    private readonly string _loggerName;

    public CustomerLogger(string name, CustomerLoggerProviderConfig config)
    {
        _config = config;
        _loggerName = name;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _config.LogLevel == logLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = $"Log {eventId}: {logLevel.ToString()} - {formatter(state, exception)}";
        WriteLoggertxt(message);
    }

    private void WriteLoggertxt(string logInfo)
    {
        string directory = @"C:\Users\PICHAU\Desktop\FACULDADE HUGO IMPORTANTE\FinanceTrack\Logs";

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        string fileName = $"log_{DateTime.Now:yyyy-MM-dd}.txt";
        string filePath = Path.Combine(directory, fileName);

        try
        {
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.WriteLine($"{DateTime.Now:HH:mm:ss} - {logInfo}");
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
}
