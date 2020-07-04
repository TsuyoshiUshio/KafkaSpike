using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.Kafka;

namespace KafkaFunctions
{
    public static class KafkaConsumer
    {
        [FunctionName("KafkaConsumer")]
        public static void Run(
            [KafkaTrigger("localhost:9092","message", ConsumerGroup = "functions")] KafkaEventData<string>[] events,
            long[] offsetArray,
            int[] partitionArray, 
            string[] topicArray,
            DateTime[] timestampArray,
            ILogger logger)
        {
            logger.LogInformation("Execution Started ======================");
            for (int i = 0; i < events.Length; i++)
            {
                logger.LogInformation($"[{timestampArray[i]}] {topicArray[i]} / {partitionArray[i]} / {offsetArray[i]}: {events[i]}");
            }
            logger.LogInformation("Execution Finished ======================");
        }
    }
}
