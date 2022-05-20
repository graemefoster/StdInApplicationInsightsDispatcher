namespace StdInAppInsightsDispatcher.CmdLineLogs;

struct Log
{
    public string Type { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public LogDetail Detail { get; set; }
}