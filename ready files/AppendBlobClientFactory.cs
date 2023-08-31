// <copyright company="Microsoft">Copyright (c) Microsoft. All rights reserved.</copyright>

namespace EventHubFuncApprepro.Blob;

using Azure.Storage.Blobs.Specialized;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Factory that Implements a custom Append Blob Client.
/// </summary>
public class AppendBlobClientFactory : BlobClientFactory
{
    /// <inheritdoc/>
    protected override IFuncBlobClient CreateBlobClient(string blobPath, IBinder binder, ILogger logger)
    {
        var appendClient = binder.Bind<AppendBlobClient>(
            new BlobAttribute(blobPath)
            {
                Connection = Literals.Datalake.ConnectionSetting,
            });

        return new FuncAppendBlobClient(appendClient, logger);
    }
}