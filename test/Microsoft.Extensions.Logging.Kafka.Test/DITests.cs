using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Microsoft.Extensions.Logging.Kafka.Test
{
    public class DITests
    {
        [Fact]
        public void KafkaClientDITests()
        {
            var services = new ServiceCollection();
            services.Configure<KafkaOptions>(options =>
            {
                options.BrokerList = "test:9092";
                options.ConsumerGroup = this.GetType().FullName;
            });

            services.AddProducer(new NullSerializer(), new ByteArraySerializer());
            services.AddConsumer(new NullDeserializer(), new ByteArrayDeserializer());

            var sp = services.BuildServiceProvider();

            Assert.IsType<Producer<Null, byte[]>>(sp.GetRequiredService<IProducer<Null, byte[]>>());
            Assert.IsType<Consumer<Null, byte[]>>(sp.GetRequiredService<IConsumer<Null, byte[]>>());
        }
    }
}
