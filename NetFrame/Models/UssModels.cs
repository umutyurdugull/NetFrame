using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetFrame.Models
{
    public class UssItem
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("size")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long? Size { get; set; }

        [JsonPropertyName("mtime")]
        public JsonElement? Mtime { get; set; }

        [JsonPropertyName("mode")]
        public string? Mode { get; set; }

        [JsonPropertyName("owner")]
        public string? Owner { get; set; }

        [JsonPropertyName("group")]
        public string? Group { get; set; }
    }

    public class UssDirectoryResponse
    {
        [JsonPropertyName("items")]
        public List<UssItem>? Items { get; set; }
    }
}
