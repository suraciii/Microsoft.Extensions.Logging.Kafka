using Microsoft.Extensions.Logging.Abstractions.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Microsoft.Extensions.Logging.Kafka
{
    public class KafkaLogger : ILogger
    {
        private readonly ChannelWriter<LogMessage> _writer;

        public KafkaLogger(ChannelWriter<LogMessage> writer, string source, string category)
        {
            _writer = writer;
            Source = source;

            Category = category;
        }

        public string Source { get; }
        public string Category { get; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter?.Invoke(state, exception);
            var logMessage = new LogMessage(Source, Category, logLevel, eventId, message, exception);

            while (true)
            {
                var vt = _writer.WaitToWriteAsync();
                var res = vt.IsCompletedSuccessfully ? vt.Result : vt.ConfigureAwait(false).GetAwaiter().GetResult();
                if (!res) return;
                if (_writer.TryWrite(logMessage))
                {
                    return;
                }
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;
    }
}
