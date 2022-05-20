using System.Net;
using Newtonsoft.Json;

namespace StdInAppInsightsDispatcher.CmdLineLogs;

struct HttpInfo
{
    public HttpStatusCode Status { get; set; }
    [JsonProperty("http_version")] public string HttpVersion { get; set; }
    public string Url { get; set; }
    public string Ip { get; set; }
    public string Method { get; set; }
    [JsonProperty("content_encoding")] public string ContentEncoding { get; set; }
}