using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetFrame.Models
{
    public class ExternalGatewayRequest
    {
        [JsonPropertyName("target")]
        public string Target { get; set; } = string.Empty;

        [JsonPropertyName("resourcePath")]
        public string ResourcePath { get; set; } = string.Empty;

        [JsonPropertyName("requestProperties")]
        public Dictionary<string, string>? RequestProperties { get; set; }

        [JsonPropertyName("timeout")]
        public int? Timeout { get; set; }

        [JsonPropertyName("wrapped")]
        public string? Wrapped { get; set; } // "Y" or "N"

        [JsonPropertyName("binary")]
        public string? Binary { get; set; } // "Y" or "N"

        [JsonPropertyName("content")]
        public object? Content { get; set; }
    }
}
