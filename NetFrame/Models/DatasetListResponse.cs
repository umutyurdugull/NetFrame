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
    }
}
