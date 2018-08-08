using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Kafka;
using Microsoft.Extensions.Options;
using System;
using BaseProducer = Confluent.Kafka.Producer<byte[], byte[]>;
using IBaseProducer = Confluent.Kafka.IProducer<byte[], byte[]>;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaClientServiceCollectionExtensions
    {

        public static IServiceCollection AddBaseProducer(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<IBaseProducer>(_ =>
            {
                var config = _.GetRequiredService<IOptions<KafkaOptions>>().Value.ToKafkaProducerConfig();
                return new BaseProducer(config, new ByteArraySerializer(), new ByteArraySerializer());
            });

            return services;
        }

        public static IServiceCollection AddProducer<TKey, TValue>(this IServiceCollection services,
            ISerializer<TKey> keySerializer,
            ISerializer<TValue> valueSerializer)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddBaseProducer();
            services.TryAddSingleton<IProducer<TKey, TValue>>(_ =>
            {
                var baseProducer = _.GetRequiredService<IBaseProducer>();
                return new Producer<TKey, TValue>(baseProducer.Handle, keySerializer, valueSerializer);
            });
            return services;
        }

        public static IServiceCollection AddConsumer<TKey, TValue>(this IServiceCollection services,
            IDeserializer<TKey> keyDeserializer,
            IDeserializer<TValue> valueDeserializer)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<IConsumer<TKey, TValue>>(_ =>
            {
                var config = _.GetRequiredService<IOptions<KafkaOptions>>().Value.ToKafkaConsumerConfig();
                return new Consumer<TKey, TValue>(config, keyDeserializer, valueDeserializer);
            });
            return services;
        }
    }
}
