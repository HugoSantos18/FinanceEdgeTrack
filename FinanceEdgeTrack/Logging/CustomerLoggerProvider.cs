using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace FinanceEdgeTrack.Logging
{
    public class CustomerLoggerProvider : ILoggerProvider
    {
        private readonly CustomerLoggerProviderConfig _config;
        private readonly ConcurrentDictionary<string, CustomerLogger> loggers
             = new ConcurrentDictionary<string, CustomerLogger>();

        public CustomerLoggerProvider(CustomerLoggerProviderConfig config)
        {
            _config = config;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, name =>new CustomerLogger(name, _config));
        }

        public void Dispose()
        {
            loggers.Clear();
        }
    }
}
