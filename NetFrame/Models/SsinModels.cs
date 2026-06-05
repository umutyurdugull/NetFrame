using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetFrame.Models.ResourceManagement
{
    public class CreateSsinRequest
    {
        [JsonPropertyName("template-id")]
        public string TemplateId { get; set; } = string.Empty;

        [JsonPropertyName("domain-id")]
        public string DomainId { get; set; } = string.Empty;

        [JsonPropertyName("tenant-id")]
        public string TenantId { get; set; } = string.Empty;

        [JsonPropertyName("registry-id")]
        public string RegistryId { get; set; } = string.Empty;

        [JsonPropertyName("quantity")]
        public string Quantity { get; set; } = "1"; // 1-7
    }

    public class SsinListResponse
    {
        [JsonPropertyName("ssin-list")]
        public List<SsinItem>? SsinList { get; set; }
    }

    public class SsinItem
    {
        [JsonPropertyName("ssin")]
        public string Ssin { get; set; } = string.Empty;

        [JsonPropertyName("provisioning-version")]
        public string? ProvisioningVersion { get; set; }
    }

    public class CreateVariableNameRequest
    {
        [JsonPropertyName("variable-prefix")]
        public string VariablePrefix { get; set; } = string.Empty;

        [JsonPropertyName("registry-id")]
        public string RegistryId { get; set; } = string.Empty;
    }

    public class VariableNameResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
