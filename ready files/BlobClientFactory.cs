// <copyright company="Microsoft">Copyright (c) Microsoft. All rights reserved.</copyright>

namespace EventHubFuncApprepro.Blob;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

/// <summary>
/// Represents an Blob Client Factory.
/// </summary>
public abstract class BlobClientFactory : IBlobClientFactory
{
    /// <summary>
    /// Creates an <see cref="IFuncBlobClient"/> for the provided <see cref="IBinder"/>.
    /// </summary>
    /// <param name="blobPath">The path of the blob to which to bind.</param>
    /// <param name="binder">An <see cref="IBinder"/> reference for which to bind client.</param>
    /// <param name="logger">An <see cref="ILogger"/>.</param>
    /// <returns>An <see cref="IFuncBlobClient"/> that can be operated.</returns>
    protected abstract IFuncBlobClient CreateBlobClient(string blobPath, IBinder binder, ILogger logger);

    /// <inheritdoc/>
    public IFuncBlobClient GetBlobClient(string blobPath, IBinder binder, ILogger logger)
    {
        var blobClient = this.CreateBlobClient(blobPath, binder, logger);
        return blobClient;
    }
}