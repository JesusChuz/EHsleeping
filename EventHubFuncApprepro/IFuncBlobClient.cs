// <copyright company="Microsoft">Copyright (c) Microsoft. All rights reserved.</copyright>

namespace EventHubFuncApprepro.Blob;

using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using static Microsoft.Azure.Amqp.Serialization.SerializableType;

/// <summary>
/// Represents an Azure Functions Blob Client.
/// </summary>
public interface IFuncBlobClient
{
    /// <summary>
    /// Azure Function is a stateless service that dynamically
    /// scales if the Event Messages load increases.
    /// To create one file per hour containing all
    /// messages read from Event Hub within that time,
    /// we need its name somewhere to be stored.
    /// The name of the one file/hour then becomes the state of this function.
    /// When Functions awake, it will attempt to read the
    /// state(one-file-name).
    /// </summary>
    /// <returns>A <see cref="Task"/> with the desired Blob File Name.</returns>
    public Task<string> GetState();

    /// <summary>
    /// Creates or Updates Internal Azure Function State.
    /// </summary>
    /// <returns>A <see cref="Task"/> task with state response.</returns>
    public Task<string> CreateOrUpdateState();

    /// <summary>
    /// Commits EventData to Azure Storage as Blob.
    /// </summary>
    /// <param name="events">Array of <see cref="EventData"/> to process.</param>
    /// <param name="partitionContext">An <see cref="PartitionContext"/> reference.</param>
    /// <returns>A <see cref="Task"/> which completes asynchronously once event processing has completed.</returns>
    public Task CommitEvents(EventData[] events, PartitionContext partitionContext);

    /// <summary>
    /// Delete a Blob.
    /// </summary>
    /// <returns>A <see cref="Task"/> which completes asynchronously once event processing has completed.</returns>
    public Task DeleteBlob();
}