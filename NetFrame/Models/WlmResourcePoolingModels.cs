using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetFrame.Models.WlmResourcePooling
{
    // --- Shared Cloud Info Model ---
    public class CloudInfo
    {
        [JsonPropertyName("domain-name")]
        public string? DomainName { get; set; }

        [JsonPropertyName("domain-id")]
        public string? DomainId { get; set; }

        [JsonPropertyName("tenant-name")]
        public string? TenantName { get; set; }

        [JsonPropertyName("tenant-id")]
        public string? TenantId { get; set; }

        [JsonPropertyName("template-name")]
        public string? TemplateName { get; set; }

        [JsonPropertyName("rdp-id")]
        public string? RdpId { get; set; }
        
        [JsonPropertyName("wrp-id")]
        public string? WrpId { get; set; }
    }

    // --- Prime WLM Resource Pool ---
    public class PrimeWrpRequest
    {
        [JsonPropertyName("cloud-info")]
        public CloudInfo CloudInfo { get; set; } = new();

        [JsonPropertyName("wrp-data")]
        public WrpData WrpData { get; set; } = new();
    }

    public class WrpData
    {
        [JsonPropertyName("wrp-name")]
        public string WrpName { get; set; } = string.Empty;

        [JsonPropertyName("service-level-agreements")]
        public List<ServiceLevelAgreement> ServiceLevelAgreements { get; set; } = new();

        [JsonPropertyName("report-class-name")]
        public string? ReportClassName { get; set; }
    }

    public class ServiceLevelAgreement
    {
        [JsonPropertyName("sla-name")]
        public string SlaName { get; set; } = string.Empty;
    }

    public class PrimeWrpResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("return-code")]
        public string? ReturnCode { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("wrp-id")]
        public string? WrpId { get; set; }

        [JsonPropertyName("wrp-name")]
        public string? WrpName { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }
    }

    // --- Delete WLM Resource Pool ---
    public class DeleteWrpResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("return-code")]
        public string? ReturnCode { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    // --- Construct WLM Service Definition ---
    public class ConstructPolicyRequest
    {
        [JsonPropertyName("cloud-info")]
        public CloudInfo CloudInfo { get; set; } = new();

        [JsonPropertyName("provision-data")]
        public ProvisionData ProvisionData { get; set; } = new();
    }

    public class ProvisionData
    {
        [JsonPropertyName("classification-rules")]
        public List<ClassificationRuleInput> ClassificationRules { get; set; } = new();
    }

    public class ClassificationRuleInput
    {
        [JsonPropertyName("service-level-agreement")]
        public string ServiceLevelAgreement { get; set; } = string.Empty;

        [JsonPropertyName("qualifier-value")]
        public string QualifierValue { get; set; } = string.Empty;
    }

    public class ConstructPolicyResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("return-code")]
        public string? ReturnCode { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("result")]
        public ConstructPolicyResult? Result { get; set; }
    }

    public class ConstructPolicyResult
    {
        [JsonPropertyName("classification-rules")]
        public List<ClassificationRuleOutput>? ClassificationRules { get; set; }
    }

    public class ClassificationRuleOutput
    {
        [JsonPropertyName("classification-rule-id")]
        public string? ClassificationRuleId { get; set; }

        [JsonPropertyName("service-class-name")]
        public string? ServiceClassName { get; set; }

        [JsonPropertyName("report-class-name")]
        public string? ReportClassName { get; set; }
    }
}
