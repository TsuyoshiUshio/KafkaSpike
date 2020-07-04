using Confluent.Kafka;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;

namespace ConsumerSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "users",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof  = true
            };

            var topic = "message";

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };


            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(topic);
                try
                {
                    var totalCount = 0;
                    while (true)
                    {
                        var c = consumer.Consume(cts.Token);
                        if(!string.IsNullOrEmpty(c.Message?.Value))
                            Console.WriteLine($"Consumed record with key {c.Message.Key} and value {c.Message.Value}");
                    }
                } catch (OperationCanceledException)
                {
                    // Ctrl-C was pressed.
                } finally
                {
                    consumer.Close();
                }

            }
        }
    }
}
