﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IoT.Common;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Newtonsoft.Json;

namespace IoT.MessageProcessor
{
    class LoggingEventProcessor : IEventProcessor
    {
        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine("LoggingEventProcessor error, partition: " +
                              $"{context.PartitionId}, error: {error.Message}");
            return Task.CompletedTask;
        }



        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine("LoggingEventProcessor opened, processing partition: " +
                              $"'{context.PartitionId}'");
            return Task.CompletedTask;
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            Console.WriteLine($"Events received on partition '{context.PartitionId}'.");

            foreach (var eventData in messages)
            {
                var payload = Encoding.ASCII.GetString(eventData.Body.Array,
                    eventData.Body.Offset,
                    eventData.Body.Count);

                var deviceId = eventData.SystemProperties["iothub-connection-device-id"];

                Console.WriteLine($"Msg received on partition '{context.PartitionId}', " +
                                  $"device ID: '{deviceId}', " +
                                  $"payload: '{payload}'");

                var telemetry = JsonConvert.DeserializeObject<Telemetry>(payload);

                if (telemetry.Status == StatusType.Emergency)
                {
                    Console.WriteLine($"someone needs emergency help! Device ID: {deviceId}");
                    SendFirstRespondersTo(telemetry.Latitude, telemetry.Longitude, telemetry.Humidity);
                }
            }
            return context.CheckpointAsync();
        }


        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine("LoggingEventProcessor closing, partition: " +
                              $"'{context.PartitionId}', reason: '{reason}'.");
            return Task.CompletedTask;
        }


        private void SendFirstRespondersTo(decimal latitude, decimal longitude, decimal humidity)
        {
            //In a real app, this is where we would send a command or notification!
            Console.WriteLine($"**First responders dispatched to ({humidity}, {latitude}, {longitude})!**");
        }
    }
}
