using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace NetFrame.Models
{
    public class UssItem
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("size")]
        public long? Size { get; set; }

        [JsonPropertyName("mtime")]
        public long? Mtime { get; set; }

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
