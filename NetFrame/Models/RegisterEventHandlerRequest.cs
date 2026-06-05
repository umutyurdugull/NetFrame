using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetFrame.Models
{
    public class RegisterEventHandlerRequest
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty; // INTERNAL or EXTERNAL

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("applId")]
        public string? ApplId { get; set; }

        [JsonPropertyName("taskId")]
        public string? TaskId { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("options")]
        public EventHandlerOptions? Options { get; set; }
    }

    public class EventHandlerOptions
    {
        [JsonPropertyName("CONTEXT_SUPPORT")]
        public string? ContextSupport { get; set; }
    }
}
