using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace NetFrame.Models
{
    public class TsoCommandRequest
    {
        [JsonPropertyName("tsoCmd")]
        public string TsoCmd { get; set; } = string.Empty;

        [JsonPropertyName("cmdState")]
        public string CmdState { get; set; } = "stateless";

        [JsonPropertyName("system")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? System { get; set; }

        [JsonPropertyName("maxWaitTime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxWaitTime { get; set; }
    }

    public class TsoCommandResponse
    {
        [JsonPropertyName("servletKey")]
        public string? ServletKey { get; set; }

        [JsonPropertyName("ver")]
        public string? Ver { get; set; }

        [JsonPropertyName("queueID")]
        public string? QueueID { get; set; }

        [JsonPropertyName("remoteSys")]
        public string? RemoteSys { get; set; }

        [JsonPropertyName("ceatsoconn")]
        public string? Ceatsoconn { get; set; }

        [JsonPropertyName("tsoData")]
        public List<TsoDataEntry>? TsoData { get; set; }

        [JsonPropertyName("appData")]
        public List<TsoDataEntry>? AppData { get; set; }

        [JsonPropertyName("timeout")]
        public bool? Timeout { get; set; }

        [JsonPropertyName("reused")]
        public bool? Reused { get; set; }

        [JsonPropertyName("msgData")]
        public List<TsoMessageEntry>? MsgData { get; set; }
    }

    public class TsoDataEntry
    {
        [JsonPropertyName("VERSION")]
        public string? Version { get; set; }

        [JsonPropertyName("DATA")]
        public string? Data { get; set; }
    }

    public class TsoMessageEntry
    {
        [JsonPropertyName("messageText")]
        public string? MessageText { get; set; }

        [JsonPropertyName("messageId")]
        public string? MessageId { get; set; }
    }
}
