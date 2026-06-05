using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetFrame.Models.CloudProvisioning
{
    public class ResourcePoolRequest<T>
    {
        [JsonPropertyName("registry-uuid")]
        public string? RegistryUuid { get; set; }

        [JsonPropertyName("template-uuid")]
        public string? TemplateUuid { get; set; }

        [JsonPropertyName("template-name")]
        public string TemplateName { get; set; } = string.Empty;

        [JsonPropertyName("tenant-id")]
        public string TenantId { get; set; } = string.Empty;

        [JsonPropertyName("network-parms")]
        public T NetworkParams { get; set; } = default!;
    }

    public class ZosmfSystem
    {
        [JsonPropertyName("sysplex-name")]
        public string SysplexName { get; set; } = string.Empty;

        [JsonPropertyName("sysplex-node-name")]
        public string SysplexNodeName { get; set; } = string.Empty;

        [JsonPropertyName("system-nickname")]
        public string SystemNickname { get; set; } = string.Empty;
    }

    public class ObtainIpParams
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("usage-type")]
        public string? UsageType { get; set; }

        [JsonPropertyName("ipaddr")]
        public string IpAddr { get; set; } = "any4";

        [JsonPropertyName("system-name")]
        public string? SystemName { get; set; }

        [JsonPropertyName("deployment-id")]
        public string? DeploymentId { get; set; }

        [JsonPropertyName("host-name")]
        public string? HostName { get; set; }

        [JsonPropertyName("system-list")]
        public List<ZosmfSystem>? SystemList { get; set; }

        [JsonPropertyName("recovery-method")]
        public string? RecoveryMethod { get; set; }

        [JsonPropertyName("application-owned")]
        public bool? ApplicationOwned { get; set; }

        [JsonPropertyName("job-name")]
        public string? JobName { get; set; }

        [JsonPropertyName("requires-zcx-addr")]
        public bool? RequiresZcxAddr { get; set; }
    }

    public class ReleaseIpParams
    {
        [JsonPropertyName("ip-id")]
        public string IpId { get; set; } = string.Empty;

        [JsonPropertyName("system-list")]
        public List<ZosmfSystem>? SystemList { get; set; }

        [JsonPropertyName("system-name")]
        public string? SystemName { get; set; }
    }

    public class ObtainIpResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("ipaddr")]
        public string IpAddr { get; set; } = string.Empty;
    }

    public class ObtainPortParams
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("usage-type")]
        public string? UsageType { get; set; }

        [JsonPropertyName("port")]
        public string? Port { get; set; }

        [JsonPropertyName("job-name")]
        public string JobName { get; set; } = string.Empty;

        [JsonPropertyName("system-name")]
        public string? SystemName { get; set; }

        [JsonPropertyName("deployment-id")]
        public string? DeploymentId { get; set; }

        [JsonPropertyName("host-name")]
        public string? HostName { get; set; }

        [JsonPropertyName("system-list")]
        public List<ZosmfSystem>? SystemList { get; set; }

        [JsonPropertyName("is-port-shared")]
        public bool? IsPortShared { get; set; }

        [JsonPropertyName("is-port-distributed")]
        public bool? IsPortDistributed { get; set; }

        [JsonPropertyName("intent")]
        public string? Intent { get; set; }
    }

    public class ReleasePortParams
    {
        [JsonPropertyName("port-id")]
        public string PortId { get; set; } = string.Empty;

        [JsonPropertyName("system-list")]
        public List<ZosmfSystem>? SystemList { get; set; }

        [JsonPropertyName("system-name")]
        public string? SystemName { get; set; }
    }

    public class ObtainPortResponse
    {
        [JsonPropertyName("id")]
        public object? Id { get; set; }

        [JsonPropertyName("port")]
        public string Port { get; set; } = string.Empty;
    }

    public class ObtainSnaParams
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("deployment-id")]
        public string? DeploymentId { get; set; }

        [JsonPropertyName("sna-appl-name")]
        public string SnaApplName { get; set; } = string.Empty;
    }

    public class ReleaseSnaParams
    {
        [JsonPropertyName("appl-name-id")]
        public string ApplNameId { get; set; } = string.Empty;
    }

    public class ObtainSnaResponse
    {
        [JsonPropertyName("id")]
        public object? Id { get; set; }

        [JsonPropertyName("appl-name")]
        public string ApplName { get; set; } = string.Empty;
    }

    // WLM Classification Rules
    public class WlmParams
    {
        [JsonPropertyName("qualifier")]
        public string? Qualifier { get; set; }

        [JsonPropertyName("cl-rule-id")]
        public string? ClRuleId { get; set; }
    }

    public class WlmClassificationRequest
    {
        [JsonPropertyName("registry-uuid")]
        public string? RegistryUuid { get; set; }

        [JsonPropertyName("template-name")]
        public string TemplateName { get; set; } = string.Empty;

        [JsonPropertyName("tenant-id")]
        public string TenantId { get; set; } = string.Empty;

        [JsonPropertyName("wlm-parms")]
        public WlmParams WlmParams { get; set; } = default!;
    }

    public class AddClassificationRuleResponse
    {
        [JsonPropertyName("cl-rule-id")]
        public string ClRuleId { get; set; } = string.Empty;

        [JsonPropertyName("report-class-name")]
        public string? ReportClassName { get; set; }

        [JsonPropertyName("service-class-name")]
        public string? ServiceClassName { get; set; }
    }

    // Storage Dataset Attributes
    public class DatasetAttributesResponse
    {
        [JsonPropertyName("dataset-attributes-list")]
        public List<DatasetAttribute>? DatasetAttributesList { get; set; }
    }

    public class DatasetAttribute
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("size")]
        public string? Size { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("data-class")]
        public string? DataClass { get; set; }

        [JsonPropertyName("storage-class")]
        public string? StorageClass { get; set; }

        [JsonPropertyName("management-class")]
        public string? ManagementClass { get; set; }

        [JsonPropertyName("volser")]
        public string? Volser { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    // LPAR Resource Pool Entry
    public class LparEntryRequest
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("group")]
        public string? Group { get; set; }

        [JsonPropertyName("complete")]
        public bool? Complete { get; set; }

        [JsonPropertyName("quiesced")]
        public bool? Quiesced { get; set; }

        [JsonPropertyName("in-use")]
        public bool? InUse { get; set; }

        [JsonPropertyName("partition-name")]
        public string? PartitionName { get; set; }

        [JsonPropertyName("partition-size")]
        public string? PartitionSize { get; set; } // "small", "medium", or "large"

        [JsonPropertyName("cpc-name")]
        public string? CpcName { get; set; }

        [JsonPropertyName("sysname")]
        public string? Sysname { get; set; }

        [JsonPropertyName("os-config-name")]
        public string? OsConfigName { get; set; }

        [JsonPropertyName("zos-volumes-list")]
        public List<ZosVolume>? ZosVolumesList { get; set; }

        [JsonPropertyName("operational-volume")]
        public string? OperationalVolume { get; set; }

        [JsonPropertyName("operational-device")]
        public string? OperationalDevice { get; set; }

        [JsonPropertyName("ipv4-ip-address")]
        public string? Ipv4IpAddress { get; set; }

        [JsonPropertyName("dns-domain-name")]
        public string? DnsDomainName { get; set; }

        [JsonPropertyName("dns-hostname")]
        public string? DnsHostname { get; set; }

        [JsonPropertyName("jes2-node-name")]
        public string? Jes2NodeName { get; set; }

        [JsonPropertyName("tcpip-vlanid")]
        public int? TcpipVlanId { get; set; }
    }

    public class LparEntryDetail : LparEntryRequest
    {
        [JsonPropertyName("lpar-pool-id")]
        public string? LparPoolId { get; set; }

        [JsonPropertyName("object-uri")]
        public string? ObjectUri { get; set; }

        [JsonPropertyName("instance-name")]
        public string? InstanceName { get; set; }

        [JsonPropertyName("rdp-name")]
        public string? RdpName { get; set; }

        [JsonPropertyName("rdp-id")]
        public string? RdpId { get; set; }

        [JsonPropertyName("rdp-job-statement")]
        public string? RdpJobStatement { get; set; }
    }

    public class ZosVolume
    {
        [JsonPropertyName("volume")]
        public string? Volume { get; set; }

        [JsonPropertyName("device")]
        public string? Device { get; set; }
    }

    public class CreateLparEntryResponse
    {
        [JsonPropertyName("lpar-pool-id")]
        public string LparPoolId { get; set; } = string.Empty;

        [JsonPropertyName("object-uri")]
        public string? ObjectUri { get; set; }
    }

    // Obtain LPAR
    public class ObtainLparRequest
    {
        [JsonPropertyName("registry-uuid")]
        public string? RegistryUuid { get; set; }

        [JsonPropertyName("template-name")]
        public string TemplateName { get; set; } = string.Empty;

        [JsonPropertyName("tenant-id")]
        public string TenantId { get; set; } = string.Empty;

        [JsonPropertyName("lpar-parms")]
        public ObtainLparParams LparParams { get; set; } = default!;
    }

    public class ObtainLparParams
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("cpc-name")]
        public string? CpcName { get; set; }

        [JsonPropertyName("partition-size")]
        public string PartitionSize { get; set; } = "any";

        [JsonPropertyName("group")]
        public string? Group { get; set; }
    }

    public class ReleaseLparRequest
    {
        [JsonPropertyName("template-name")]
        public string TemplateName { get; set; } = string.Empty;

        [JsonPropertyName("tenant-id")]
        public string TenantId { get; set; } = string.Empty;

        [JsonPropertyName("registry-uuid")]
        public string? RegistryUuid { get; set; }

        [JsonPropertyName("lpar-parms")]
        public ReleaseLparParams LparParams { get; set; } = default!;
    }

    public class ReleaseLparParams
    {
        [JsonPropertyName("lpar-pool-id")]
        public string LparPoolId { get; set; } = string.Empty;
    }
}
