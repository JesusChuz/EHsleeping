// <copyright company="Microsoft">Copyright (c) Microsoft. All rights reserved.</copyright>

namespace EventHubFuncApprepro.Blob;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Helper class to create path of the blob to which to bind.
/// </summary>
public static class AdlsBlobHelper
{
    /// <summary>
    /// The Path where the state of this Azure Functions is stored.
    /// 
    /// Context: Azure Function is a stateless service that dynamically
    /// scales if the Event Messages load increases.
    /// To create one file per hour containing all
    /// messages read from Event Hub within that time,
    /// we need its name somewhere to be stored.
    /// The name of the one file/hour then becomes the state of this function.
    /// </summary>
    /// <param name="hour">To add a fractional number of hours to utcNow</param>
    /// <returns>A string of the format containerName/yyyy/MM/dd_HH_state.json</returns>
    public static string StatePath(int hour = 0)
    {
        var date = DateTime.UtcNow.AddHours(hour);
        var blobName = Literals.Datalake.StateContainerName;
        return $"{blobName}/{date:yyyy}/{date:MM}/{date:dd}_{date:HH}_state.txt";
    }

    /// <summary>
    /// Azure Storage Path to commit the Blob Content read from EventHub.
    /// </summary>
    /// <param name="fileName">Name of EventHub file data file name.</param>
    /// <param name="hour">To add a fractional number of hours to utcNow</param>
    /// <returns>A string of the format containerName/yyyy/MM/dd/HH/fileName.json</returns>
    public static string CommitPath(string fileName, int hour = 0)
    {
        var date = DateTime.UtcNow.AddHours(hour);
        var blobName = Environment.GetEnvironmentVariable(Literals.Datalake.ContainerName);
        return $"{blobName}/{date:yyyy}/{date:MM}/{date:dd}/{date:HH}/{fileName}.json";
    }
}