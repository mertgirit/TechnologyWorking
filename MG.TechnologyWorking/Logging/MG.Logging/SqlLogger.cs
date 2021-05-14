using System;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace MG.Logging
{
    using MG.DAL.EntityFrameworkCore;

    public class SqlLogger : ILogger
    {
        private string connectionString;
        public SqlLogger(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            try
            {
                DbContextOptionsBuilder<MGContext> optionsBuilder = new DbContextOptionsBuilder<MGContext>();
                optionsBuilder.UseSqlServer(connectionString, options => options.EnableRetryOnFailure());

                using (var context = new MGContext(optionsBuilder.Options))
                {
                    context.Database.AutoTransactionsEnabled = false;
                    context.Logs.Add(new Models.DataModels.Logs
                    {
                        Application = eventId.Id > 0 ? eventId.Id.ToString() : "0",
                        ApplicationService = !string.IsNullOrEmpty(eventId.Name) ? eventId.Name : "",
                        CreateDate = DateTime.Now,
                        LogLevel = logLevel.ToString(),
                        Message = state.ToString(),
                        ErrorMessage = exception?.Message == null ? "" : exception?.Message,
                        StackTrace = exception?.StackTrace == null ? "" : exception?.StackTrace,
                    });
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                //TODO MG:?
            }
        }
    }
}