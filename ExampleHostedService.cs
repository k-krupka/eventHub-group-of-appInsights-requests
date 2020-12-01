using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Hosting;

namespace eventHub_group_of_appInsights_requests
{
    public class ExampleHostedService : IHostedService
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly EventProcessorClient _processor;

        public ExampleHostedService(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;

            var storageClient = new BlobContainerClient(
                Configuration.StorageConnectionString,
                Configuration.BlobContainerName);

            _processor = new EventProcessorClient(
                storageClient,
                "$Default",
                Configuration.EventHubsConnectionString,
                Configuration.EventHubName,
                new EventProcessorClientOptions
                {
                    LoadBalancingStrategy = LoadBalancingStrategy.Greedy
                });
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("StartAsync called...");

            Console.WriteLine("Created Clients. Registering handler...");

            _processor.ProcessEventAsync += ProcessEventHandler;
            _processor.PartitionInitializingAsync += ProcessorOnPartitionInitializingAsync;
            _processor.ProcessErrorAsync += ProcessorOnProcessErrorAsync;
            _processor.PartitionClosingAsync += ProcessorOnPartitionClosingAsync;

            return _processor.StartProcessingAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _processor.StopProcessingAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _processor.ProcessEventAsync -= ProcessEventHandler;
                _processor.PartitionInitializingAsync -= ProcessorOnPartitionInitializingAsync;
                _processor.ProcessErrorAsync -= ProcessorOnProcessErrorAsync;
                _processor.PartitionClosingAsync -= ProcessorOnPartitionClosingAsync;
            }
        }

        private Task ProcessEventHandler(ProcessEventArgs args)
        {
            if (args.CancellationToken.IsCancellationRequested)
            {
                return Task.CompletedTask;
            }

            using (_telemetryClient.StartOperation<RequestTelemetry>("This is a request telemetry for individual received EventData"))
            {
                _telemetryClient.TrackTrace("example log which falls into this RequestTelemetry");
            }

            return Task.CompletedTask;
        }

        private Task ProcessorOnPartitionClosingAsync(PartitionClosingEventArgs arg)
        {
            return Task.CompletedTask;
        }

        private Task ProcessorOnProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            Console.WriteLine($"error: {arg.Exception.Message}");

            return Task.CompletedTask;
        }

        private Task ProcessorOnPartitionInitializingAsync(PartitionInitializingEventArgs arg)
        {
            Console.WriteLine($"Initializing partition {arg.PartitionId}");

            return Task.CompletedTask;
        }
    }
}