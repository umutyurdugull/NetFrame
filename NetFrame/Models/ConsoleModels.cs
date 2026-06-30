using System.Text.Json.Serialization;

namespace NetFrame.Models
{
    public class ConsoleCommandRequest
    {
        [JsonPropertyName("cmd")]
        public string Cmd { get; set; } = string.Empty;

        [JsonPropertyName("system")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? System { get; set; }
    }

    public class ConsoleCommandResponse
    {
        [JsonPropertyName("cmd-response-url")]
        public string? CmdResponseUrl { get; set; }
    }

    public class ConsoleResponseMessages
    {
        [JsonPropertyName("cmd-response")]
        public string? CmdResponse { get; set; }
    }
}
