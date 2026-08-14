using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetFrame.Models
{
    public class DatasetListResponse
    {
        [JsonPropertyName("items")]
        public List<DatasetListItem>? Items { get; set; }
    }

    public class DatasetListItem
    {
        [JsonPropertyName("dsname")]
        public string? DsName { get; set; }

        [JsonPropertyName("dsorg")]
        public string? Dsorg { get; set; }

        [JsonPropertyName("recfm")]
        public string? Recfm { get; set; }

        [JsonPropertyName("lrecl")]
        public int? Lrecl { get; set; }

        [JsonPropertyName("blksize")]
        public int? Blksize { get; set; }

        [JsonPropertyName("volser")]
        public string? Volser { get; set; }
    }
}
