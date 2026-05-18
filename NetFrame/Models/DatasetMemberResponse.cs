using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetFrame.Models
{
    public class DatasetMemberResponse
    {
        [JsonPropertyName("items")]
        public List<DatasetMemberItem> Items { get; set; } = new();

        [JsonPropertyName("returnedRows")]
        public int ReturnedRows { get; set; }

        [JsonPropertyName("totalRows")]
        public int? TotalRows { get; set; }

        [JsonPropertyName("JSONversion")]
        public int JsonVersion { get; set; }
    }

    public class DatasetMemberItem
    {
        [JsonPropertyName("member")]
        public string Member { get; set; } = string.Empty;

        [JsonPropertyName("vers")]
        public int? Vers { get; set; }

        [JsonPropertyName("mod")]
        public int? Mod { get; set; }

        [JsonPropertyName("c4date")]
        public string? C4Date { get; set; }

        [JsonPropertyName("m4date")]
        public string? M4Date { get; set; }

        [JsonPropertyName("cnorc")]
        public int? Cnorc { get; set; }

        [JsonPropertyName("inorc")]
        public int? Inorc { get; set; }

        [JsonPropertyName("mnorc")]
        public int? Mnorc { get; set; }

        [JsonPropertyName("mtime")]
        public string? MTime { get; set; }

        [JsonPropertyName("msec")]
        public string? MSec { get; set; }

        [JsonPropertyName("user")]
        public string? User { get; set; }

        [JsonPropertyName("sclm")]
        public string? Sclm { get; set; }
    }
}
