using Newtonsoft.Json;

namespace StdInAppInsightsDispatcher.CmdLineLogs;

struct Operation
{
    [JsonProperty("query_execution_time")] public double QueryExecutionTime { get; set; }

    [JsonProperty("user_vars")] public Dictionary<string, string> UserVars { get; set; }
    [JsonProperty("request_id")] public string RequestId { get; set; }
    [JsonProperty("response_size")] public int ResponseSize { get; set; }
    [JsonProperty("request_mode")] public string RequestMode { get; set; }
    [JsonProperty("request_read_time")] public string RequestReadTime { get; set; }
    [JsonProperty("query")] public QueryInfo? Query { get; set; }

}