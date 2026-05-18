
using System.Text.Json.Serialization;

public class ZosmfInfoResponse()
{
    [JsonPropertyName("zos_version")]
    public string ZosVersion { get; set; }

    [JsonPropertyName("api_version")]
    public string ApiVersion { get; set; }
}