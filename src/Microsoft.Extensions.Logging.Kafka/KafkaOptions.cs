using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.Logging.Kafka
{

    public class KafkaOptions
    {
        public string BrokerList { get; set; }

        /// <summary>
        /// Default timeout for network requests. 
        /// default 5s
        /// </summary>
        public TimeSpan SocketTimeout { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// librdkafka statistics emit interval.
        /// default zero - DISABLED
        /// </summary>
        public TimeSpan StatisticsInterval { get; set; } = TimeSpan.Zero;

        public string SecurityProtocol { get; set; }
        public string SASLMechanisms { get; set; }
        public string SASL_Username { get; set; }
        public string SASL_Password { get; set; }

        /// <summary>
        /// Maximum total message size sum allowed on the producer queue.
        /// default 64mb
        /// </summary>
        public int QueueBufferingMaxKbytes { get; set; } = 1024 * 64; /* 256mb */

        #region consumer options

        public string ConsumerGroup { get; set; }
        /// <summary>
        /// Maximum number of kilobytes per topic+partition in the local consumer queue. 
        /// default 64mb
        /// </summary>
        public int QueuedMaxMessagesKbytes { get; set; } = 1024 * 64; /* 64mb */

        #endregion

    }

    public static class KafkaConfigurationExtensions
    {
        public static Dictionary<string, object> ToKafkaProducerConfig(this KafkaOptions kafkaOptions)
        {
            var config = new Dictionary<string, object>
            {
                ["bootstrap.servers"] = kafkaOptions.BrokerList,

                ["security.protocol"] = kafkaOptions.SecurityProtocol,
                ["sasl.mechanism"] = kafkaOptions.SASLMechanisms,
                ["sasl.username"] = kafkaOptions.SASL_Username,
                ["sasl.password"] = kafkaOptions.SASL_Password,

                ["socket.timeout.ms"] = kafkaOptions.SocketTimeout.TotalMilliseconds,
                ["statistics.interval.ms"] = kafkaOptions.StatisticsInterval.TotalMilliseconds,
                ["queue.buffering.max.kbytes"] = kafkaOptions.QueueBufferingMaxKbytes,
                ["queue.buffering.max.ms"] = 64,
                ["log.connection.close"] = true,

                ["socket.blocking.max.ms"] = 1,
                ["compression.codec"] = "lz4",
                ["socket.nagle.disable"] = true,
                ["request.required.acks"] = 1,

            };
            config = config.Where(kvp => kvp.Value != null).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return config;
        }

        public static Dictionary<string, object> ToKafkaConsumerConfig(this KafkaOptions kafkaOptions)
        {
            var config = new Dictionary<string, object>
            {
                ["group.id"] = kafkaOptions.ConsumerGroup,
                ["enable.auto.commit"] = true,
                ["auto.commit.interval.ms"] = 3000,
                ["bootstrap.servers"] = kafkaOptions.BrokerList,

                ["security.protocol"] = kafkaOptions.SecurityProtocol,
                ["sasl.mechanism"] = kafkaOptions.SASLMechanisms,
                ["sasl.username"] = kafkaOptions.SASL_Username,
                ["sasl.password"] = kafkaOptions.SASL_Password,

                ["socket.timeout.ms"] = kafkaOptions.SocketTimeout.TotalMilliseconds,
                ["statistics.interval.ms"] = kafkaOptions.StatisticsInterval.TotalMilliseconds,
                ["queued.max.messages.kbytes"] = kafkaOptions.QueuedMaxMessagesKbytes,
                ["auto.offset.reset"] = "smallest",

                ["log.connection.close"] = false,
                ["socket.blocking.max.ms"] = 1,
                ["socket.nagle.disable"] = true,
            };
            config = config.Where(kvp => kvp.Value != null).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return config;
        }
    }
}
