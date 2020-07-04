using Avro.Generic;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AvroSample
{
    class Program
    {
        async static Task ProduceGeneric(string brokers, string schemaRegistryUrl)
        {
            using(var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig { Url = schemaRegistryUrl}))
            using (var producer =
                new ProducerBuilder<Null, GenericRecord>(new ProducerConfig { BootstrapServers = brokers })
                .SetValueSerializer(new AvroSerializer<GenericRecord>(schemaRegistry))
                .Build())
            {
                var logLevelSchema = (Avro.EnumSchema)Avro.Schema.Parse(
                    File.ReadAllText("LogLevel.asvc"));
                var logMessageSchema = (Avro.RecordSchema)Avro.Schema.Parse(
                    File.ReadAllText("LogMessage.V1.asvc")
                    .Replace(
                        "MessageTypes.LogLevel",
                        File.ReadAllText("LogLevel.asvc")));

                var record = new GenericRecord(logMessageSchema);
                record.Add("IP", "127.0.0.1");
                record.Add("Message", "a test log message");
                record.Add("Severity", new GenericEnum(logLevelSchema, "Error"));

                await producer
                    .ProduceAsync("log-messages", new Message<Null, GenericRecord> { Value = record })
                    .ContinueWith(task => Console.WriteLine(
                        task.IsFaulted
                        ? $"error producing message: {task.Exception.Message}"
                        : $"produced to: {task.Result.TopicPartitionOffset}"));
                producer.Flush(TimeSpan.FromSeconds(30));
            }
        }

        async static Task ProduceSpecific(string bootstrapServers, string schemaRegistryUrl)
        {
            using (var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig { Url = schemaRegistryUrl }))
            using (var producer = new ProducerBuilder<Null, LogMessage>(new ProducerConfig { BootstrapServers = bootstrapServers })
                .SetValueSerializer(new AvroSerializer<LogMessage>(schemaRegistry))
                .Build())
            {
                await producer.ProduceAsync("log-messages",
                    new Message<Null, LogMessage>
                    {
                        Value = new LogMessage
                        {
                            IP = "192.168.0.1",
                            Message = "a test message 2",
                            Severity = LogLevel.Info,
                            Tags = new Dictionary<string, string> { { "location", "CA" } }
                        }
                    });
                producer.Flush(TimeSpan.FromSeconds(30));
            }
                
            
        }
    }
}
