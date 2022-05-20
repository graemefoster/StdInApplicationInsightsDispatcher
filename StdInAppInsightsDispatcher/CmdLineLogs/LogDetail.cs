using Newtonsoft.Json;

namespace StdInAppInsightsDispatcher.CmdLineLogs;

struct LogDetail
{
    public string Kind { get; set; }
    public Operation? Operation { get; set; }
    [JsonProperty("request_id")] public string? RequestId { get; set; }
    [JsonProperty("http_info")] public HttpInfo? HttpInfo { get; set; }
    [JsonProperty("query")] public QueryInfo? Query { get; set; }
}