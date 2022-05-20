// See https://aka.ms/new-console-template for more information

using System.Net;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Newtonsoft.Json;
using StdInAppInsightsDispatcher.CmdLineLogs;

var cancellation = new CancellationTokenSource();
var telemetryClient = new TelemetryClient(new TelemetryConfiguration
{
    ConnectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")
});
var dispatcher = DispatchStandardInputToApplicationInsights(cancellation.Token, Console.OpenStandardInput(), telemetryClient);
await dispatcher;

async Task DispatchStandardInputToApplicationInsights(CancellationToken token, Stream stdInStream, TelemetryClient client)
{
    var now = DateTimeOffset.Now;
    using var stdIn = new StreamReader(stdInStream);
    var keepGoing = true;
    await Task.Yield();

    while (!stdIn.EndOfStream && !token.IsCancellationRequested && keepGoing)
    {
        try
        {
            var input = await stdIn.ReadLineAsync();
            if (input != null)
            {
                await ProcessLog(input, client, now);
            }
            else
            {
                await Task.Delay(100);
            }
        }
        catch (ObjectDisposedException)
        {
            keepGoing = false;
        }
    }
}

Task<bool> ProcessLog(string msg, TelemetryClient telemetryClient, DateTimeOffset processStartTime)
{
    Console.WriteLine(msg);
    if (msg.Contains("\"type\":\"unstructured\"")) return Task.FromResult(false);
    var log = JsonConvert.DeserializeObject<Log>(msg);

    if (log.Timestamp < processStartTime) return Task.FromResult(false);

    if (log.Type == "http-log")
    {
        telemetryClient.TrackRequest(new RequestTelemetry("GQL request", log.Timestamp,
            TimeSpan.FromSeconds(log.Detail.Operation!.Value.QueryExecutionTime),
            log.Detail.HttpInfo!.Value.Status.ToString(),
            log.Detail.HttpInfo!.Value.Status == HttpStatusCode.OK)
        {
            Context = { Operation = { Id = log.Detail.RequestId, Name = log.Detail.Operation!.Value.Query?.OperationName ?? log.Detail.HttpInfo!.Value.Url} },
            Timestamp = log.Timestamp,
        });
        return Task.FromResult(true);
    }

    if (log.Type == "query-log" && log.Detail.Kind == "database")
    {
        telemetryClient.TrackDependency(
            new DependencyTelemetry("Database", "Postgres", "Query", log.Detail.Query!.Value.Query)
            {
                Context = { Operation = { Id = log.Detail.RequestId, Name = log.Detail.Query?.OperationName } },
                Timestamp = log.Timestamp
            });
        return Task.FromResult(true);
    }
    
    return Task.FromResult(false);
}