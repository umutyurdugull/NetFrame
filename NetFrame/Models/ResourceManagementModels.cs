using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetFrame.Models.ResourceManagement
{
    // Domain Models
    public class DomainListResponse
    {
        [JsonPropertyName("domain-list")]
        public List<DomainDetail>? DomainList { get; set; }

        [JsonPropertyName("local-system")]
        public DomainSystem? LocalSystem { get; set; }
    }

    public class DomainDetail
    {
        [JsonPropertyName("domain-id")]
        public string? DomainId { get; set; }

        [JsonPropertyName("domain-name")]
        public string? DomainName { get; set; }

        [JsonPropertyName("domain-state")]
        public string? DomainState { get; set; }

        [JsonPropertyName("domain-system-list")]
        public List<DomainSystem>? DomainSystemList { get; set; }

        [JsonPropertyName("domain-administrator-list")]
        public List<string>? DomainAdministratorList { get; set; }

        [JsonPropertyName("network-administrator-list")]
        public List<string>? NetworkAdministratorList { get; set; }

        [JsonPropertyName("wlm-administrator-list")]
        public List<string>? WlmAdministratorList { get; set; }

        [JsonPropertyName("security-administrator")]
        public string? SecurityAdministrator { get; set; }

        [JsonPropertyName("security-job-statement")]
        public string? SecurityJobStatement { get; set; }

        [JsonPropertyName("automatic-security")]
        public bool? AutomaticSecurity { get; set; }

        [JsonPropertyName("domain-approver-list")]
        public List<string>? DomainApproverList { get; set; }

        [JsonPropertyName("object-uri")]
        public string? ObjectUri { get; set; }

        [JsonPropertyName("domain-description")]
        public string? DomainDescription { get; set; }

        [JsonPropertyName("create-time")]
        public string? CreateTime { get; set; }

        [JsonPropertyName("created-by-user")]
        public string? CreatedByUser { get; set; }

        [JsonPropertyName("last-modified-time")]
        public string? LastModifiedTime { get; set; }

        [JsonPropertyName("last-modified-by-user")]
        public string? LastModifiedByUser { get; set; }

        [JsonPropertyName("SAF-resources")]
        public List<SafResource>? SafResources { get; set; }

        [JsonPropertyName("domain-shared-rdp-id")]
        public string? DomainSharedRdpId { get; set; }
    }

    public class DomainSystem
    {
        [JsonPropertyName("sysplex-name")]
        public string? SysplexName { get; set; }

        [JsonPropertyName("sysplex-node-name")]
        public string? SysplexNodeName { get; set; }

        [JsonPropertyName("system-nickname")]
        public string? SystemNickname { get; set; }
    }

    public class SafResource
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("ids")]
        public List<string>? Ids { get; set; }

        [JsonPropertyName("groups")]
        public List<string>? Groups { get; set; }

        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("resource-class")]
        public string? ResourceClass { get; set; }

        [JsonPropertyName("resource-name")]
        public string? ResourceName { get; set; }

        [JsonPropertyName("required-access")]
        public string? RequiredAccess { get; set; }
    }

    public class DomainHistoryResponse
    {
        [JsonPropertyName("history")]
        public List<HistoryObject>? History { get; set; }
    }

    public class HistoryObject
    {
        [JsonPropertyName("action-type")]
        public string? ActionType { get; set; }

        [JsonPropertyName("user")]
        public string? User { get; set; }

        [JsonPropertyName("action-time")]
        public string? ActionTime { get; set; }

        [JsonPropertyName("action-details")]
        public string? ActionDetails { get; set; }
    }

    // Tenant Models
    public class CreateTenantRequest
    {
        [JsonPropertyName("tenant-name")]
        public string TenantName { get; set; } = string.Empty;

        [JsonPropertyName("tenant-description")]
        public string? TenantDescription { get; set; }

        [JsonPropertyName("tenant-consumer-list")]
        public List<string>? TenantConsumerList { get; set; }

        [JsonPropertyName("tenant-group-list")]
        public List<string>? TenantGroupList { get; set; }

        [JsonPropertyName("tenant-metering-capping-properties")]
        public TenantMeteringCappingProperties? MeteringCappingProperties { get; set; }

        [JsonPropertyName("tenant-managed-by")]
        public TenantManagedBy? ManagedBy { get; set; }
    }

    public class TenantMeteringCappingProperties
    {
        [JsonPropertyName("tenant-capping-enabled")]
        public bool? TenantCappingEnabled { get; set; }

        [JsonPropertyName("tenant-cpu-cap-limit")]
        public int? TenantCpuCapLimit { get; set; }

        [JsonPropertyName("tenant-cpu-cap-type")]
        public string? TenantCpuCapType { get; set; } // lpar-share-percentage, service-unit, cp, msu

        [JsonPropertyName("tenant-memory-cap-limit")]
        public int? TenantMemoryCapLimit { get; set; }

        [JsonPropertyName("tenant-memory-capping-enabled")]
        public bool? TenantMemoryCappingEnabled { get; set; }

        [JsonPropertyName("tenant-metering-enabled")]
        public bool? TenantMeteringEnabled { get; set; }

        [JsonPropertyName("tenant-solution-id")]
        public string? TenantSolutionId { get; set; }
    }

    public class TenantManagedBy
    {
        [JsonPropertyName("system")]
        public DomainSystem? System { get; set; }

        [JsonPropertyName("tenant-id")]
        public string? TenantId { get; set; }

        [JsonPropertyName("tenant-name")]
        public string? TenantName { get; set; }

        [JsonPropertyName("zosmf-url")]
        public string? ZosmfUrl { get; set; }
    }

    public class CreateTenantResponse
    {
        [JsonPropertyName("tenant-id")]
        public string? TenantId { get; set; }

        [JsonPropertyName("object-uri")]
        public string? ObjectUri { get; set; }

        [JsonPropertyName("tenant-domain-id")]
        public string? TenantDomainId { get; set; }
    }

    public class TenantDetail : CreateTenantResponse
    {
        [JsonPropertyName("tenant-name")]
        public string? TenantName { get; set; }

        [JsonPropertyName("tenant-shared-rdp-id")]
        public string? TenantSharedRdpId { get; set; }

        [JsonPropertyName("tenant-domain-name")]
        public string? TenantDomainName { get; set; }

        [JsonPropertyName("tenant-state")]
        public string? TenantState { get; set; }

        [JsonPropertyName("tenant-metering-capping-properties")]
        public TenantMeteringCappingProperties? MeteringCappingProperties { get; set; }

        [JsonPropertyName("tenant-templates")]
        public List<TenantTemplate>? TenantTemplates { get; set; }

        [JsonPropertyName("tenant-consumer-list")]
        public List<string>? TenantConsumerList { get; set; }

        [JsonPropertyName("tenant-group-list")]
        public List<string>? TenantGroupList { get; set; }

        [JsonPropertyName("tenant-description")]
        public string? TenantDescription { get; set; }

        [JsonPropertyName("create-time")]
        public string? CreateTime { get; set; }

        [JsonPropertyName("created-by-user")]
        public string? CreatedByUser { get; set; }

        [JsonPropertyName("last-modified-time")]
        public string? LastModifiedTime { get; set; }

        [JsonPropertyName("last-modified-by-user")]
        public string? LastModifiedByUser { get; set; }

        [JsonPropertyName("SAF-resources")]
        public List<SafResource>? SafResources { get; set; }
    }

    public class TenantTemplate
    {
        [JsonPropertyName("template-name")]
        public string? TemplateName { get; set; }

        [JsonPropertyName("rdp-id")]
        public string? RdpId { get; set; }

        [JsonPropertyName("template-available")]
        public bool? TemplateAvailable { get; set; }
    }

    public class TenantListResponse
    {
        [JsonPropertyName("tenant-list")]
        public List<TenantDetail>? TenantList { get; set; }
    }

    // Action Request Models
    public class AssignCpuCappingRequest
    {
        [JsonPropertyName("tenant-cpu-cap-limit")]
        public int TenantCpuCapLimit { get; set; }

        [JsonPropertyName("tenant-cpu-cap-type")]
        public string TenantCpuCapType { get; set; } = string.Empty;
    }

    public class AssignMemoryCappingRequest
    {
        [JsonPropertyName("tenant-memory-cap-limit")]
        public int TenantMemoryCapLimit { get; set; }
    }

    public class AssignSolutionIdRequest
    {
        [JsonPropertyName("tenant-solution-id")]
        public string TenantSolutionId { get; set; } = string.Empty;
    }

    public class TenantConsumerActionRequest
    {
        [JsonPropertyName("tenant-consumer-list")]
        public List<string> TenantConsumerList { get; set; } = new();
    }

    public class TenantGroupActionRequest
    {
        [JsonPropertyName("tenant-group-list")]
        public List<string> TenantGroupList { get; set; } = new();
    }

    public class TenantDescriptionActionRequest
    {
        [JsonPropertyName("tenant-description")]
        public string TenantDescription { get; set; } = string.Empty;
    }

    // Resource Pool Models
    public class ResourcePoolDetail
    {
        [JsonPropertyName("rdp-id")]
        public string? RdpId { get; set; }

        [JsonPropertyName("rdp-name")]
        public string? RdpName { get; set; }

        [JsonPropertyName("rdp-pool-type")]
        public string? RdpPoolType { get; set; }

        [JsonPropertyName("rdp-domain-id")]
        public string? RdpDomainId { get; set; }

        [JsonPropertyName("rdp-tenant-id")]
        public string? RdpTenantId { get; set; }

        [JsonPropertyName("rdp-template-name")]
        public string? RdpTemplateName { get; set; }

        [JsonPropertyName("rdp-instance-limit")]
        public int? RdpInstanceLimit { get; set; }

        [JsonPropertyName("rdp-instance-actual")]
        public int? RdpInstanceActual { get; set; }

        [JsonPropertyName("rdp-ready")]
        public bool? RdpReady { get; set; }

        [JsonPropertyName("rdp-quiesced")]
        public bool? RdpQuiesced { get; set; }
        
        // ... many more properties can be added if needed based on the response
    }
}
