using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetFrame.Models.Provisioning
{
    // --- Shared Models ---

    public class PromptVariable
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("required")]
        public bool? Required { get; set; }

        [JsonPropertyName("label")]
        public string? Label { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; } // boolean, string, integer, etc.

        [JsonPropertyName("choices")]
        public List<string>? Choices { get; set; }

        [JsonPropertyName("regex")]
        public string? Regex { get; set; }
    }

    public class ApprovalObject
    {
        [JsonPropertyName("object-id")]
        public string? ObjectId { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; } // pending, approved, rejected

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("approvers")]
        public List<string>? Approvers { get; set; }

        [JsonPropertyName("run-as-user")]
        public string? RunAsUser { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; } // general, domain, action_definition, step_definition
    }

    public class ActionObject
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("is-deprovision")]
        public string? IsDeprovision { get; set; }

        [JsonPropertyName("command")]
        public string? Command { get; set; }

        [JsonPropertyName("instructions")]
        public string? Instructions { get; set; }
    }

    public class CompositeDefinition
    {
        [JsonPropertyName("sequence")]
        public int Sequence { get; set; }

        [JsonPropertyName("number-of-instances")]
        public int NumberOfInstances { get; set; }

        [JsonPropertyName("published-template-name")]
        public string PublishedTemplateName { get; set; } = string.Empty;

        [JsonPropertyName("connectors")]
        public List<Connector>? Connectors { get; set; }

        [JsonPropertyName("prompt-variables")]
        public List<PromptVariable>? PromptVariables { get; set; }
        
        [JsonPropertyName("missing")]
        public bool? Missing { get; set; }
    }

    public class Connector
    {
        [JsonPropertyName("variable-name")]
        public string VariableName { get; set; } = string.Empty;

        [JsonPropertyName("source-template")]
        public string SourceTemplate { get; set; } = string.Empty;

        [JsonPropertyName("source-variable-name")]
        public string SourceVariableName { get; set; } = string.Empty;
    }

    // --- Private Catalog (scc) Models ---

    public class SoftwareTemplateRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("template-type")]
        public string? TemplateType { get; set; } // standard, composite

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("domain-name")]
        public string? DomainName { get; set; }

        // Standard Template specific
        [JsonPropertyName("action-definition-file")]
        public string? ActionDefinitionFile { get; set; }

        [JsonPropertyName("workflow-definition-file")]
        public string? WorkflowDefinitionFile { get; set; }

        [JsonPropertyName("workflow-variable-input-file")]
        public string? WorkflowVariableInputFile { get; set; }

        // Composite Template specific
        [JsonPropertyName("composite-definition")]
        public List<CompositeDefinition>? CompositeDefinition { get; set; }

        [JsonPropertyName("composite-cluster")]
        public bool? CompositeCluster { get; set; }

        [JsonPropertyName("approvals")]
        public List<string>? Approvals { get; set; }
    }

    public class SoftwareTemplateDetail : SoftwareTemplateRequest
    {
        [JsonPropertyName("object-id")]
        public string? ObjectId { get; set; }

        [JsonPropertyName("generated-name")]
        public string? GeneratedName { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("owner")]
        public string? Owner { get; set; }

        [JsonPropertyName("version")]
        public string? Version { get; set; }

        [JsonPropertyName("tenants")]
        public List<string>? Tenants { get; set; }

        [JsonPropertyName("actions")]
        public List<ActionObject>? Actions { get; set; }

        [JsonPropertyName("prompt-variables")]
        public List<PromptVariable>? PromptVariables { get; set; }

        [JsonPropertyName("create-time")]
        public string? CreateTime { get; set; }
    }

    public class TemplateListResponse
    {
        [JsonPropertyName("scc-list")]
        public List<SoftwareTemplateDetail>? SccList { get; set; }
        
        [JsonPropertyName("psc-list")]
        public List<SoftwareTemplateDetail>? PscList { get; set; }
    }

    public class TemplateHistoryResponse
    {
        [JsonPropertyName("history")]
        public List<TemplateHistoryObject>? History { get; set; }
    }

    public class TemplateHistoryObject
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

    // --- Execution (Run/Test) Models ---

    public class RunTemplateRequest
    {
        [JsonPropertyName("input-variables")]
        public List<RuntimeProperty>? InputVariables { get; set; }

        [JsonPropertyName("domain-name")]
        public string? DomainName { get; set; }

        [JsonPropertyName("tenant-name")]
        public string? TenantName { get; set; }

        [JsonPropertyName("account-info")]
        public string? AccountInfo { get; set; }

        [JsonPropertyName("systems-nicknames")]
        public List<string>? SystemsNicknames { get; set; }
    }

    public class RuntimeProperty
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
    }

    public class RunTemplateResponse
    {
        [JsonPropertyName("registry-info")]
        public RegistryInfo? RegistryInfo { get; set; }

        [JsonPropertyName("workflow-info")]
        public WorkflowInfo? WorkflowInfo { get; set; }

        [JsonPropertyName("composite-children-registry-info")]
        public List<RegistryInfo>? CompositeChildrenRegistryInfo { get; set; }

        [JsonPropertyName("system-nickname")]
        public string? SystemNickname { get; set; }
    }

    public class RegistryInfo
    {
        [JsonPropertyName("object-id")]
        public string? ObjectId { get; set; }

        [JsonPropertyName("object-name")]
        public string? ObjectName { get; set; }

        [JsonPropertyName("object-uri")]
        public string? ObjectUri { get; set; }

        [JsonPropertyName("external-name")]
        public string? ExternalName { get; set; }
    }

    public class WorkflowInfo
    {
        [JsonPropertyName("workflowKey")]
        public string? WorkflowKey { get; set; }

        [JsonPropertyName("workflowID")]
        public string? WorkflowId { get; set; }
    }
}
