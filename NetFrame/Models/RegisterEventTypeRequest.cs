using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetFrame.Models
{
    public class RegisterEventTypeRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("desc")]
        public string? Description { get; set; }

        [JsonPropertyName("owner")]
        public string Owner { get; set; } = string.Empty;

        [JsonPropertyName("params")]
        public Dictionary<string, string>? Parameters { get; set; }
    }
}
