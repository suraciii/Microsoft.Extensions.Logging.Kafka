using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Microsoft.Extensions.Logging.Kafka
{
    public class ICHLoggerOptions
    {

        /// <summary>
        /// Logger source
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// task for consuming logs, default to 1
        /// </summary>
        public int ReaderCount { get; set; } = 1;

        /// <summary>
        /// queue capacity, -1 for unlimit, default to 100000
        /// </summary>
        public int ChannelCapacity { get; set; } = 100000;

        /// <summary>
        /// Behaviour when queue is full, default to DropWrite
        /// Wait --> wait to queue
        /// DropNewest --> drop the newest log in queue
        /// DropOldest --> drop the oldest log in queue
        /// DropWrite --> drop the log which is ready to enqueue
        /// </summary>
        public BoundedChannelFullMode ChannelFullMode { get; set; } = BoundedChannelFullMode.DropWrite;

        /// <summary>
        /// The kafka topic for logging
        /// </summary>
        public string TopicName { get; set; }

    }
}
