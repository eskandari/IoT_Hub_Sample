using System;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace IoT.MessageProcessor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hubName = "XX";
            var iotHubConnectionString = "XX";
            var storageConnectionString = "XX";
            var consumerGroupName = PartitionReceiver.DefaultConsumerGroupName;
            var storageContainerName = "Ehsan";

            var processor = new EventProcessorHost(
            hubName,
            consumerGroupName,
            iotHubConnectionString,
            storageConnectionString,
            storageContainerName);

            await processor.RegisterEventProcessorAsync<LoggingEventProcessor>();

            Console.WriteLine("Event processor started, press enter to exit...");

            Console.ReadLine();

            await processor.UnregisterEventProcessorAsync();

        }
    }
}
