using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Logging.Kafka
{
    public interface ILogMessageProducer
    {
        void Produce(LogMessage message);
        void Flush();
    }
}
