
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
        string path = @"C:\Users\PICHAU\Desktop\FACULDADE HUGO IMPORTANTE\FinanceTrack\Logs";


        using (StreamWriter arquivoLog = new StreamWriter(path, true))
        {
            try
            {
                arquivoLog.WriteLine(logInfo);
                arquivoLog.Close();
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
