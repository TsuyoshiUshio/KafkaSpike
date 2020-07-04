using Confluent.Kafka;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace KafkaSpike
{
    class Program
    {
        static void Main(string broker, string topic, int thread = 2, int iteration = 10)
        {
            if (string.IsNullOrEmpty(broker))
            {
                Console.WriteLine("--broker required.");
                Environment.Exit(1);
                return;
            }
            if (string.IsNullOrEmpty(topic))
            {
                Console.WriteLine("--topic required.");
                Environment.Exit(1);
                return;
            }
            Execute(broker, topic, thread, iteration).GetAwaiter().GetResult();
        }

        private static async Task Execute(string broker, string topic, int thread, int iteration)
        {
            var config = new ProducerConfig { BootstrapServers = broker };
            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    Func<int, Task> lambda = async (int m) =>
                    {
                        for (var i = 0; i < iteration; i++)
                        {
                            var dr = await p.ProduceAsync(topic, new Message<Null, string> { Value = $"Produced by ProducerSample :{i}:{m}" });
                            Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                        }
                    };
                    Task[] tasks = new Task[thread];
                    for (var m = 0; m < thread; m++)
                    {
                        tasks[m] = lambda(m);
                    }

                    await Task.WhenAll(tasks);

                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
            Console.WriteLine("Press enter key to stop this app");
            Console.ReadLine();
        }
    }
}
