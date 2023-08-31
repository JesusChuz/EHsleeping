// <copyright company="Microsoft">Copyright (c) Microsoft. All rights reserved.</copyright>

namespace EventHubFuncApprepro.Functions;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using EventHubFuncApprepro.Blob;

/// <summary>
/// The Azure Function to Execute.
/// </summary>
public class EventHubToBlob
{
    private static readonly ActivitySource Source = new ($"{typeof(EventHubToBlob)}");
    private static readonly Meter Meter = new ($"{typeof(EventHubToBlob)}");
    private static readonly Counter<long> ExceptionThrown = Meter.CreateCounter<long>("custom.runtime.dotnet.exceptions.count");

    private readonly IBlobClientFactory blobClientFactory;

    /// <summary>
    /// Initializes a new instance of <see cref="EventHubToBlob"/>.
    /// </summary>
    /// <param name="factory">A <see cref="BlobClientFactory"/> used to create blob Client.</param>
    public EventHubToBlob(IBlobClientFactory factory)
    {
        this.blobClientFactory = factory;
    }

    /// <summary>
    /// This method is the entry point for the time-triggered Azure Function.
    /// It runs on a predefined schedule specified by the TimerTrigger attribute.
    /// </summary>
    /// <param name="myTimer">Time trigger information.</param>
    /// <param name="log">ILogger instance for logging within the function.</param>
    [FunctionName("TimeTriggerFunction")]
    public void TimeTrigger(
        [TimerTrigger("0 */2 * * * *")] TimerInfo myTimer,
        ILogger log)
    {
        // The TimeTrigger attribute specifies a CRON expression: "0 */2 * * * *"

        // This means the function will be triggered every 2 minutes (*/2) starting from 0 seconds past the minute (0).
        // The other asterisks (*) denote any valid value for the other time components.
        log.LogInformation($"TimeTrigger function executed at: {DateTime.Now}");
    }

    /// <summary>
    /// Function to Process an Array of <see cref="EventData"/> and write to a Blob Storage.
    /// </summary>
    /// <param name="events">Array of <see cref="EventData"/> to process.</param>
    /// <param name="partitionContext">An <see cref="PartitionContext"/> reference.</param>
    /// <param name="binder">An <see cref="IBinder"/> reference for which to bind client.</param>
    /// <param name="log">An <see cref="ILogger"/>.</param>
    /// <returns>A <see cref="Task"/> which completes asynchronously once event processing has completed.</returns>
    [FunctionName("PrimaryEventHubToBlobFunction")]
    public async Task Primary(
        [
            EventHubTrigger(
                Literals.PrimaryEventHub.Name,
                Connection = Literals.PrimaryEventHub.ConnectionSetting,
                ConsumerGroup = Literals.PrimaryEventHub.ConsumerGroupName)
        ] EventData[] events,
        PartitionContext partitionContext,
        IBinder binder,
        ILogger log)
    {
        await this.WriteEventDataToAdls(events, partitionContext, binder, log);
    }

    /// <summary>
    /// Function to Process an Array of <see cref="EventData"/> and write to a Blob Storage.
    /// </summary>
    /// <param name="events">Array of <see cref="EventData"/> to process.</param>
    /// <param name="partitionContext">An <see cref="PartitionContext"/> reference.</param>
    /// <param name="binder">An <see cref="IBinder"/> reference for which to bind client.</param>
    /// <param name="log">An <see cref="ILogger"/>.</param>
    /// <returns>A <see cref="Task"/> which completes asynchronously once event processing has completed.</returns>
    [FunctionName("SecondaryEventHubToBlobFunction")]
    public async Task Secondary(
        [
            EventHubTrigger(
                Literals.SecondaryEventHub.Name,
                Connection = Literals.SecondaryEventHub.ConnectionSetting,
                ConsumerGroup = Literals.SecondaryEventHub.ConsumerGroupName)
        ] EventData[] events,
        PartitionContext partitionContext,
        IBinder binder,
        ILogger log)
    {
        await this.WriteEventDataToAdls(events, partitionContext, binder, log);
    }

    private async Task WriteEventDataToAdls(
        EventData[] events,
        PartitionContext partitionContext,
        IBinder binder,
        ILogger log)
    {
        using var activity = Source.StartActivity($"{nameof(this.WriteEventDataToAdls)}");

        // Create Client pointing to state path.
        IFuncBlobClient fileNameBlobClient = this.blobClientFactory.GetBlobClient(
            AdlsBlobHelper.StatePath(),
            binder,
            log);

        try
        {
            var commitPathFileName = await fileNameBlobClient.GetState();

            // Create Client pointing to CommitPath.
            IFuncBlobClient commitBlobClient = this.blobClientFactory.GetBlobClient(
                AdlsBlobHelper.CommitPath(commitPathFileName),
                binder,
                log);

            await commitBlobClient.CommitEvents(events, partitionContext);
        }
        catch (RequestFailedException ex)
        {
            // https://learn.microsoft.com/en-us/rest/api/storageservices/blob-service-error-codes
            // We cannot insert more than 50k entries in one blob.
            // We will delete the blob and recreate to update its state.
            if (ex.ErrorCode == "BlockCountExceedsLimit")
            {
                await fileNameBlobClient.DeleteBlob();
            }
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);

            throw;
        }
        finally
        {
            ExceptionThrown.Add(1);
        }
    }
}