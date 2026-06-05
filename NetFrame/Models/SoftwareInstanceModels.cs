using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetFrame.Models.Provisioning
{
    public class SoftwareInstanceRequest
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("registry-type")]
        public string RegistryType { get; set; } = "catalog"; // catalog or general

        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;

        [JsonPropertyName("catalog-object-id")]
        public string? CatalogObjectId { get; set; }

        [JsonPropertyName("catalog-object-name")]
        public string? CatalogObjectName { get; set; }

        [JsonPropertyName("external-name")]
        public string? ExternalName { get; set; }

        [JsonPropertyName("system-nickname")]
        public string? SystemNickname { get; set; }

        [JsonPropertyName("owner")]
        public string? Owner { get; set; }

        [JsonPropertyName("domain-id")]
        public string? DomainId { get; set; }

        [JsonPropertyName("tenant-id")]
        public string? TenantId { get; set; }

        [JsonPropertyName("variables")]
        public List<InstanceVariable>? Variables { get; set; }

        [JsonPropertyName("actions")]
        public List<InstanceAction>? Actions { get; set; }
    }

    public class InstanceVariable
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("visibility")]
        public string? Visibility { get; set; } // public or private

        [JsonPropertyName("update-registry")]
        public string? UpdateRegistry { get; set; } // "true" or "false"
    }

    public class InstanceAction
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty; // command, workflow, instructions

        [JsonPropertyName("is-deprovision")]
        public string? IsDeprovision { get; set; } // "true" or "false"

        [JsonPropertyName("command")]
        public string? Command { get; set; }

        [JsonPropertyName("instructions")]
        public string? Instructions { get; set; }
    }

    public class SoftwareInstanceDetail : SoftwareInstanceRequest
    {
        [JsonPropertyName("object-id")]
        public string? ObjectId { get; set; }

        [JsonPropertyName("object-name")]
        public string? ObjectName { get; set; }

        [JsonPropertyName("workflow-key")]
        public string? WorkflowKey { get; set; }

        [JsonPropertyName("created-time")]
        public string? CreatedTime { get; set; }

        [JsonPropertyName("last-modified-time")]
        public string? LastModifiedTime { get; set; }

        [JsonPropertyName("last-action-name")]
        public string? LastActionName { get; set; }

        [JsonPropertyName("system")]
        public string? System { get; set; }

        [JsonPropertyName("sysplex")]
        public string? Sysplex { get; set; }
    }

    public class InstanceListResponse
    {
        [JsonPropertyName("scr-list")]
        public List<SoftwareInstanceDetail>? SccList { get; set; }
    }

    public class PerformActionRequest
    {
        [JsonPropertyName("input-variables")]
        public List<RuntimeProperty>? InputVariables { get; set; }

        [JsonPropertyName("target-system-nickname")]
        public string? TargetSystemNickname { get; set; }
    }

    public class ActionResult
    {
        [JsonPropertyName("action-id")]
        public string? ActionId { get; set; }

        [JsonPropertyName("action-uri")]
        public string? ActionUri { get; set; }
        
        [JsonPropertyName("state")]
        public string? State { get; set; }
        
        [JsonPropertyName("ran-at-time")]
        public string? RanAtTime { get; set; }
    }

    public class ActionResponseList
    {
        [JsonPropertyName("scr-list-actions")]
        public List<ActionResult>? Actions { get; set; }
    }
}
