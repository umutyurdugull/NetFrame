using System.Text.Json.Serialization;

namespace NetFrame.Models
{
    public class CreateDatasetRequest
    {
        [JsonPropertyName("volser")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Volser { get; set; }

        [JsonPropertyName("unit")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Unit { get; set; }

        [JsonPropertyName("dsorg")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Dsorg { get; set; }

        [JsonPropertyName("alcunit")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Alcunit { get; set; }

        [JsonPropertyName("primary")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Primary { get; set; }

        [JsonPropertyName("secondary")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Secondary { get; set; }

        [JsonPropertyName("dirblk")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Dirblk { get; set; }

        [JsonPropertyName("avgblk")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Avgblk { get; set; }

        [JsonPropertyName("recfm")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Recfm { get; set; }

        [JsonPropertyName("blksize")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Blksize { get; set; }

        [JsonPropertyName("lrecl")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Lrecl { get; set; }

        [JsonPropertyName("storclass")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Storclass { get; set; }

        [JsonPropertyName("mgntclass")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Mgntclass { get; set; }

        [JsonPropertyName("dataclass")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Dataclass { get; set; }

        [JsonPropertyName("dsntype")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Dsntype { get; set; }

        [JsonPropertyName("like")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Like { get; set; }
    }
}