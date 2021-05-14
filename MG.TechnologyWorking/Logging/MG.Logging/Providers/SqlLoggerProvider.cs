using Microsoft.Extensions.Logging;

namespace MG.Logging.Providers
{
    public class SqlLoggerProvider : ILoggerProvider
    {
        private string connectionString;
        public SqlLoggerProvider(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new SqlLogger(connectionString);
        }

        public void Dispose()
        {
        }
    }
}