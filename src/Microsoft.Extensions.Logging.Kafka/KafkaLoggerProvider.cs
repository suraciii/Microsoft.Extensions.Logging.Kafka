using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Logging.Kafka
{
    [ProviderAlias("Kafka")]
    public class KafkaLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, KafkaLogger> _loggers = new ConcurrentDictionary<string, KafkaLogger>();

        private readonly Channel<LogMessage> _channel;
        private readonly ILogMessageProducer _producer;
        private readonly ICHLoggerOptions _options;
        private readonly CancellationTokenSource _tokenSource;
        private readonly CancellationToken _token;
        private readonly List<Task> readerTasks = new List<Task>();

        public KafkaLoggerProvider(Channel<LogMessage> channel, ILogMessageProducer producer, IOptions<ICHLoggerOptions> options)
        {
            _options = options?.Value;
            _channel = channel;
            _producer = producer;

            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;

            for (int i = 0; i < _options.ReaderCount; i++)
            {
                readerTasks.Add(Task.Run(Polling));
            }
        }

        private async Task Polling()
        {
            try
            {
                var reader = _channel.Reader;
                while (true)
                {
                    if (!await reader.WaitToReadAsync(_token).ConfigureAwait(false))
                    {
                        return;
                    }
                    if (reader.TryRead(out var item))
                    {
                        _producer.Produce(item);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, CreateLoggerImplementation);
        }

        private KafkaLogger CreateLoggerImplementation(string categoryName)
        {
            return new KafkaLogger(_channel.Writer, _options.Source, categoryName);
        }

        public async Task Flush()
        {
            Console.WriteLine("Flush channel");
            _channel.Writer.Complete();
            var readerComp = _channel.Reader.Completion;
            if (!readerComp.IsCompleted)
                await _channel.Reader.Completion;
            await Task.WhenAll(readerTasks);
        }


        public void Dispose()
        {
            try
            {
                Flush().GetAwaiter().GetResult();
            }
            finally
            {
                Console.WriteLine("Flush producer");
                _producer.Flush();
            }
        }
    }
}
