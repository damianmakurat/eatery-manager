using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace eatery_manager
{
    public class TextBoxLogger : ILogger
    {
        private readonly Action<string> _writeLog;

        public TextBoxLogger(Action<string> writeLog)
        {
            _writeLog = writeLog;
        }

        public IDisposable BeginScope<TState>(TState state) => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                var logMessage = formatter(state, exception);
                _writeLog?.Invoke($"[{logLevel}] {logMessage}");
            }
        }
    }

    public class TextBoxLoggerProvider : ILoggerProvider
    {
        private readonly Action<string> _writeLog;
        public TextBoxLoggerProvider(Action<string> writeLog) => _writeLog = writeLog;
        public ILogger CreateLogger(string categoryName) => new TextBoxLogger(_writeLog);
        public void Dispose() { }
    }

}
