using Confluent.Kafka;
using MessagePack;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.Logging.Kafka
{
    public class LogMessageProducer : ILogMessageProducer
    {
        private readonly string _loggingTopic;
        private readonly IProducer<Null, byte[]> _producer;
        public LogMessageProducer(IProducer<Null, byte[]> producer, IOptions<ICHLoggerOptions> options)
        {
            _loggingTopic = options?.Value.TopicName ?? throw new ArgumentNullException("topic name");
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _producer.OnError += (s, e) => Console.WriteLine(e.ToString());
        }

        public void Produce(LogMessage message)
        {
            var msg = MessagePackSerializer.Serialize(message);
            var kafkaMessage = new Message<Null, byte[]>
            {
                Key = null,
                Value = msg,
                Timestamp = new Timestamp(message.Timestamp),
            };

            while (true)
            {
                try
                {
                    _producer.BeginProduce(_loggingTopic, kafkaMessage, null);
                    return;
                }
                catch (KafkaException ke) when (ke.Error.Code == ErrorCode.Local_QueueFull)
                {
                    continue;
                }
            }
        }

        public void Flush()
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
        }
    }
}
