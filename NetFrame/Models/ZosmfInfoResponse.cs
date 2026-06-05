
using System.Text.Json.Serialization;

namespace NetFrame.Models
{
    public class ZosmfInfoResponse
    {
        [JsonPropertyName("zos_version")]
        public string? ZosVersion { get; set; }

        [JsonPropertyName("api_version")]
        public string? ApiVersion { get; set; }

        [JsonPropertyName("zosmf_version")]
        public string? ZosmfVersion { get; set; }

        [JsonPropertyName("zosmf_full_version")]
        public string? ZosmfFullVersion { get; set; }

        [JsonPropertyName("zosmf_saf_realm")]
        public string? ZosmfSafRealm { get; set; }

        [JsonPropertyName("zosmf_port")]
        public string? ZosmfPort { get; set; }

        [JsonPropertyName("zosmf_hostname")]
        public string? ZosmfHostname { get; set; }
    }
}