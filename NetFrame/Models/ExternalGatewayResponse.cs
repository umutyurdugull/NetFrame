using System.Text.Json.Serialization;

namespace NetFrame.Models
{
    public class ExternalGatewayResponse
    {
        [JsonPropertyName("primaryAPIVersion")]
        public string? PrimaryAPIVersion { get; set; }

        [JsonPropertyName("systemsOutput")]
        public SystemsOutput? SystemsOutput { get; set; }

        [JsonPropertyName("numOfSystems")]
        public int? NumOfSystems { get; set; }
    }

    public class SystemsOutput
    {
        [JsonPropertyName("systemOutput")]
        public object? SystemOutput { get; set; }

        [JsonPropertyName("rc")]
        public string? ReturnCode { get; set; } // "Ok", "HttpConnectionFailed", etc.

        [JsonPropertyName("error")]
        public ExternalGatewayError? Error { get; set; }

        [JsonPropertyName("systemName")]
        public string? SystemName { get; set; }
    }

    public class ExternalGatewayError
    {
        [JsonPropertyName("msgid")]
        public string? MsgId { get; set; }

        [JsonPropertyName("msgtxt")]
        public string? MsgText { get; set; }
    }
}
