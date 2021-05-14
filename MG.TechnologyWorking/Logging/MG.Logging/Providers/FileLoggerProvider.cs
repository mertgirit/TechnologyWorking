using Microsoft.Extensions.Logging;

namespace MG.Logging.Providers
{
    public class FileLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger();
        }

        public void Dispose()
        {
        }
    }
}