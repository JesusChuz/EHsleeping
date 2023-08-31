// <copyright company="Microsoft">Copyright (c) Microsoft. All rights reserved.</copyright>

namespace EventHubFuncApprepro.Blob;

using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Custom Implementation for the Append Blob Client
/// In order to interact with EventHub messages and
/// store them in an Azure Data Lake Storage.
/// </summary>
public class FuncAppendBlobClient : IFuncBlobClient
{
    private static readonly ActivitySource Source = new ($"{typeof(FuncAppendBlobClient)}");
    private readonly AppendBlobClient appendClient;
    private readonly ILogger log;

    /// <summary>
    /// Initializes a new instance of <see cref="FuncAppendBlobClient"/>.
    /// </summary>
    /// <param name="blobClient">A <see cref="AppendBlobClient"/> client.</param>
    /// <param name="log">An <see cref="ILogger"/>.</param>
    public FuncAppendBlobClient(AppendBlobClient blobClient, ILogger log)
    {
        this.appendClient = blobClient;
        this.log = log;
    }

    /// <inheritdoc/>
    public async Task<string> GetState()
    {
        using var activity = Source.StartActivity($"{nameof(this.GetState)}");

        // If state does not exist, it will throw
        try
        {
            BlobDownloadResult downloadResult = await this.appendClient.DownloadContentAsync();

            if (downloadResult != null)
            {
                return downloadResult.Content.ToString();
            }
        }
        catch (Exception)
        {
        }

        return await this.CreateOrUpdateState();
    }

    /// <inheritdoc/>
    public async Task<string> CreateOrUpdateState()
    {
        // Create State
        // State will be equal to Commit Blob Path.
        try
        {
            var state = $"0_{Guid.NewGuid()}_1";
            await this.UploadString(state);
            return state;
        }
        catch (Exception ex)
        {
            this.log.LogError(ex, message: $"{nameof(this.GetState)} Failed.");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteBlob()
    {
        await this.appendClient.DeleteIfExistsAsync();
    }

    /// <inheritdoc/>
    public async Task CommitEvents(EventData[] events, PartitionContext partitionContext)
    {
        _ = events ?? throw new ArgumentNullException(nameof(events));

        using var activity = Source.StartActivity($"{nameof(this.CommitEvents)}");

        if (events.Length == 0)
        {
            this.log.LogInformation("No events to commit.");
            return;
        }

        try
        {
            foreach (var eventData in events)
            {
                dynamic eventBody = JsonConvert.DeserializeObject(eventData.EventBody.ToString());

                eventBody.EventProcessedUtcTime = DateTime.UtcNow;
                eventBody.PartitionId = partitionContext.PartitionId;
                eventBody.EventEnqueuedUtcTime = eventData.EnqueuedTime;

                var content = $"{JsonConvert.SerializeObject(eventBody)}\n";

                await this.UploadString(content);
            }
        }
        catch (Exception ex)
        {
            this.log.LogError(ex, message: $"{nameof(this.CommitEvents)} Failed.");
            throw;
        }
    }

    private async Task UploadString(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            throw new ArgumentNullException(nameof(content));
        }

        try
        {
            await this.appendClient.CreateIfNotExistsAsync();
            await this.appendClient.AppendBlockAsync(StringToStream(content));
        }
        catch (Exception ex)
        {
            this.log.LogError(ex, message: $"{nameof(this.UploadString)} Failed.");
            throw;
        }
    }

    private static MemoryStream StringToStream(string content)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        MemoryStream stream = new (bytes);
        return stream;
    }
}