// <copyright company="Microsoft">Copyright (c) Microsoft. All rights reserved.</copyright>

using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter.Geneva;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using EventHubFuncApprepro;
//using EventHubToAdls;
//using RecurrenceProcessor.EventHubToAdls.Blob;

[assembly: FunctionsStartup(typeof(Startup))]

namespace EventHubFuncApprepro;

/// <summary>
/// The Startup Class Overrides FunctionsStartup
/// to add Observability and Factory dependencies
/// through Injection Pattern.
/// </summary>
public class Startup : FunctionsStartup
{
    /// <inheritdoc/>
    public override void Configure(IFunctionsHostBuilder builder)
    {
        //builder.Services.AddLogging(
        //    configure => configure.AddOpenTelemetry(options =>
        //    {
        //        options.AddGenevaLogExporter(exporterOptions =>
        //        {
        //            exporterOptions.ConnectionString = "EtwSession=OpenTelemetry";
        //        });
        //    }));

        //var metricsAccount = Environment.GetEnvironmentVariable(Literals.Geneva.MetricsAccount);
        //var metricsNamespace = Literals.Geneva.EventHubToAdlsMetricsNamespace;

        //// https://github.com/open-telemetry/opentelemetry-dotnet-contrib/blob/main/src/OpenTelemetry.Instrumentation.Runtime/README.md#runtime-instrumentation-for-opentelemetry-net
        //builder.Services.AddOpenTelemetryMetrics(
        //    builder => builder
        //        .AddRuntimeInstrumentation()
        //        .AddMeter("*")
        //        .AddGenevaMetricExporter(options =>
        //        {
        //            options.ConnectionString = $"Account={metricsAccount};Namespace={metricsNamespace}";
        //        })
        //    );

        //// https://eng.ms/docs/products/geneva/collect/instrument/opentelemetrydotnet/otel-traces#what-fields-are-exported-by-the-geneva-exporter
        //builder.Services.AddOpenTelemetryTracing(
        //    builder => builder
        //        .SetSampler(new AlwaysOnSampler())
        //        .AddSource("*")
        //        .AddAspNetCoreInstrumentation()
        //        .AddHttpClientInstrumentation()
        //        .AddGenevaTraceExporter(options =>
        //        {
        //            options.ConnectionString = "EtwSession=OpenTelemetry";
        //        }));

        //builder.Services.AddSingleton<IBlobClientFactory, AppendBlobClientFactory>();
    }
}