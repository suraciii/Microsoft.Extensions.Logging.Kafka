using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Logging.Kafka
{
    [MessagePackObject]
    public class LogMessage
    {
        public LogMessage(string source,
            string category,
            LogLevel logLevel,
            EventId eventId,
            string message) : this(source, category, logLevel, eventId, message, null)
        { }
        public LogMessage(string source,
            string category,
            LogLevel logLevel,
            EventId eventId,
            string message,
            Exception exception) : this(source, category, logLevel, eventId.Id, eventId.Name, message, exception?.ToString(), DateTimeOffset.Now)
        { }

        public LogMessage(string source,
            string category,
            LogLevel logLevel,
            int eventId,
            string eventName,
            string message,
            string exception,
            DateTimeOffset timestamp)
        {
            Source = source;
            LogLevel = logLevel;
            Category = category;
            EventId = eventId;
            EventName = eventName;
            Message = message;
            Exception = exception;
            Timestamp = timestamp;
        }

        [Key(0)]
        public string Source { get; set; }
        [Key(1)]
        public string Category { get; set; }
        [Key(2)]
        public LogLevel LogLevel { get; set; }
        [Key(3)]
        public int EventId { get; set; }
        [Key(4)]
        public string EventName { get; set; }
        [Key(5)]
        public string Message { get; set; }
        [Key(6)]
        public string Exception { get; set; }
        [Key(7)]
        public DateTimeOffset Timestamp { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
