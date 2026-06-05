using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace NetFrame.Models
{
    public class AppLinkingResponse<T>
    {
        [JsonPropertyName("error")]
        public AppLinkingError? Error { get; set; }

        [JsonPropertyName("result")]
        public T? Result { get; set; }
    }

    public class AppLinkingError
    {
        [JsonPropertyName("msgid")]
        public string? MsgId { get; set; }

        [JsonPropertyName("msgtxt")]
        public string? MsgText { get; set; }
    }

    public class EligibleTasksResult
    {
        [JsonPropertyName("Task")]
        public List<EligibleTask>? Tasks { get; set; }
    }

    public class EligibleTask
    {
        [JsonPropertyName("TaskID")]
        public string? TaskId { get; set; }

        [JsonPropertyName("navigationUrl")]
        public string? NavigationUrl { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("PluginID")]
        public string? PluginId { get; set; }
    }

    public class HandlerDetail
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("taskId")]
        public string? TaskId { get; set; }

        [JsonPropertyName("enabled")]
        public bool? Enabled { get; set; }

        [JsonPropertyName("defaultHandler")]
        public bool? DefaultHandler { get; set; }

        [JsonPropertyName("applId")]
        public string? ApplId { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("eventTypeId")]
        public string? EventTypeId { get; set; }

        [JsonPropertyName("options")]
        public EventHandlerOptions? Options { get; set; }
    }
}
