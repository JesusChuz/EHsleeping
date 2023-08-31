// <copyright company="Microsoft">Copyright (c) Microsoft. All rights reserved.</copyright>

namespace EventHubFuncApprepro;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Constants for the Functions Project.
/// </summary>
public static class Literals
{
    /// <summary>
    /// Geneva Constants.
    /// </summary>
    public static class Geneva
    {
        /// <summary>
        /// The Monitoring Account.
        /// </summary>
        public const string MonitoringAccount = "MONITORING_GCS_ACCOUNT";

        /// <summary>
        /// The Monitoring Namespace.
        /// </summary>
        public const string MonitoringNamespace = "MONITORING_GCS_NAMESPACE";

        /// <summary>
        /// The Metrics Account.
        /// </summary>
        public const string MetricsAccount = "Metric_GCS_ACCOUNT";

        /// <summary>
        /// The EventHub To ADLS Namespace.
        /// </summary>
        public const string EventHubToAdlsMetricsNamespace = "EventHubToAdls";
    }

    /// <summary>
    /// Primary Event Hub Constants.
    /// </summary>
    public static class PrimaryEventHub
    {
        /// <summary>
        /// Prefix that Client will use to look up MSI metadata in Function App Settings
        /// </summary>
        public const string ConnectionSetting = "%PRIMARY_EVENTHUB_CONNECTION%";

        /// <summary>
        /// https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-event-hubs-trigger?tabs=in-process%2Cfunctionsv2%2Cextensionv5&pivots=programming-language-csharp#attributes
        /// </summary>
        public const string Name = "%PRIMARY_EVENTHUB_NAME%";

        /// <summary>
        /// Cannot be consumed from App Settings.
        /// </summary>
        public const string ConsumerGroupName = "eventhubtoadls";
    }

    /// <summary>
    /// Secondary EventHub Constants.
    /// </summary>
    public static class SecondaryEventHub
    {
        /// <summary>
        /// Prefix that Client will use to look up MSI metadata in Function App Settings
        /// </summary>
        public const string ConnectionSetting = "%SECONDARY_EVENTHUB_CONNECTION%";

        /// <summary>
        /// https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-event-hubs-trigger?tabs=in-process%2Cfunctionsv2%2Cextensionv5&pivots=programming-language-csharp#attributes
        /// </summary>
        public const string Name = "%SECONDARY_EVENTHUB_NAME%";

        /// <summary>
        /// Cannot be consumed from App Settings.
        /// </summary>
        public const string ConsumerGroupName = "eventhubtoadls";
    }

    /// <summary>
    /// Data Lake Constants.
    /// </summary>
    public static class Datalake
    {
        /// <summary>
        /// Prefix that Client will use to look up MSI metadata in Function App Settings
        /// </summary>
        public const string ConnectionSetting = "%BLOB_CONTAINER_CONNECTION%";

        /// <summary>
        /// The Data Lake Output Container Name.
        /// </summary>
        public const string ContainerName = "BLOB_CONTAINER_NAME";

        /// <summary>
        /// The Data Lake State Container Name.
        /// </summary>
        public const string StateContainerName = "recproc-eventhub-stage";
    }
}