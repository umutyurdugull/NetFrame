using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NetFrame.Models.Workflow
{
    public class CreateWorkflowRequest
    {
        [JsonPropertyName("workflowName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? WorkflowName { get; set; }

        [JsonPropertyName("workflowDefinitionFile")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? WorkflowDefinitionFile { get; set; }

        [JsonPropertyName("system")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? System { get; set; }

        [JsonPropertyName("owner")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Owner { get; set; }

        [JsonPropertyName("assignToOwner")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? AssignToOwner { get; set; }

        [JsonPropertyName("accessType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AccessType { get; set; }

        [JsonPropertyName("deleteCompletedJobs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? DeleteCompletedJobs { get; set; }

        [JsonPropertyName("autoDeleteOnCompletion")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? AutoDeleteOnCompletion { get; set; }
        
        [JsonExtensionData]
        public Dictionary<string, System.Text.Json.JsonElement>? ExtensionData { get; set; }
    }

    public class CreateWorkflowResponse
    {
        [JsonPropertyName("workflowKey")]
        public string? WorkflowKey { get; set; }

        [JsonPropertyName("workflowDescription")]
        public string? WorkflowDescription { get; set; }

        [JsonPropertyName("workflowID")]
        public string? WorkflowId { get; set; }

        [JsonPropertyName("workflowVersion")]
        public string? WorkflowVersion { get; set; }

        [JsonPropertyName("vendor")]
        public string? Vendor { get; set; }
    }

    public class WorkflowProperties
    {
        [JsonPropertyName("workflowKey")]
        public string? WorkflowKey { get; set; }

        [JsonPropertyName("workflowName")]
        public string? WorkflowName { get; set; }

        [JsonPropertyName("statusName")]
        public string? StatusName { get; set; }

        [JsonExtensionData]
        public Dictionary<string, System.Text.Json.JsonElement>? ExtensionData { get; set; }
    }

    public class ListWorkflowsResponse
    {
        [JsonPropertyName("workflows")]
        public List<WorkflowProperties>? Workflows { get; set; }
    }

    public class StartWorkflowRequest
    {
        [JsonPropertyName("stepName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? StepName { get; set; }

        [JsonPropertyName("performSubsequent")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? PerformSubsequent { get; set; }

        [JsonExtensionData]
        public Dictionary<string, System.Text.Json.JsonElement>? ExtensionData { get; set; }
    }
    
    public class CancelWorkflowResponse
    {
        [JsonPropertyName("workflowName")]
        public string? WorkflowName { get; set; }
    }
    
    public class ArchiveWorkflowResponse
    {
        [JsonPropertyName("workflowKey")]
        public string? WorkflowKey { get; set; }
    }

    public class ListArchivedWorkflowsResponse
    {
        [JsonPropertyName("archivedWorkflows")]
        public List<ArchivedWorkflowInfo>? ArchivedWorkflows { get; set; }
    }

    public class ArchivedWorkflowInfo
    {
        [JsonPropertyName("workflowName")]
        public string? WorkflowName { get; set; }

        [JsonPropertyName("workflowKey")]
        public string? WorkflowKey { get; set; }

        [JsonPropertyName("archivedInstanceURI")]
        public string? ArchivedInstanceUri { get; set; }
    }
}
