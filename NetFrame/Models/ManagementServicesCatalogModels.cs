using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NetFrame.Models.ManagementServicesCatalog
{
    public class CatalogServiceSummary
    {
        [JsonPropertyName("csName")]
        public string? CsName { get; set; }

        [JsonPropertyName("objectId")]
        public string? ObjectId { get; set; }

        [JsonPropertyName("csDescription")]
        public string? CsDescription { get; set; }

        [JsonPropertyName("csCategoryName")]
        public string? CsCategoryName { get; set; }

        [JsonPropertyName("csState")]
        public string? CsState { get; set; }

        [JsonExtensionData]
        public Dictionary<string, System.Text.Json.JsonElement>? ExtensionData { get; set; }
    }

    public class Category
    {
        [JsonPropertyName("objectId")]
        public string? ObjectId { get; set; }

        [JsonPropertyName("ccName")]
        public string? CcName { get; set; }

        [JsonPropertyName("ccDescription")]
        public string? CcDescription { get; set; }

        [JsonExtensionData]
        public Dictionary<string, System.Text.Json.JsonElement>? ExtensionData { get; set; }
    }

    public class ServiceSubmissionSummary
    {
        [JsonPropertyName("siName")]
        public string? SiName { get; set; }

        [JsonPropertyName("objectId")]
        public string? ObjectId { get; set; }

        [JsonPropertyName("siStatus")]
        public string? SiStatus { get; set; }

        [JsonPropertyName("siTargetSystem")]
        public string? SiTargetSystem { get; set; }

        [JsonExtensionData]
        public Dictionary<string, System.Text.Json.JsonElement>? ExtensionData { get; set; }
    }

    public class CreateServiceSubmissionRequest
    {
        [JsonPropertyName("siCatalogServiceId")]
        public string SiCatalogServiceId { get; set; } = string.Empty;

        [JsonPropertyName("siTargetSystem")]
        public string? SiTargetSystem { get; set; }

        [JsonPropertyName("siRunAutomatically")]
        public bool? SiRunAutomatically { get; set; }

        [JsonPropertyName("siChangeRecord")]
        public string? SiChangeRecord { get; set; }

        [JsonPropertyName("siComment")]
        public string? SiComment { get; set; }

        [JsonPropertyName("siRunAfter")]
        public string? SiRunAfter { get; set; }

        [JsonPropertyName("siExpires")]
        public string? SiExpires { get; set; }

        [JsonPropertyName("siInputs")]
        public JsonObject? SiInputs { get; set; }

        [JsonPropertyName("siJobstatement")]
        public string? SiJobStatement { get; set; }
    }

    public class ModifyServiceSubmissionRequest
    {
        [JsonPropertyName("siInputs")]
        public JsonObject? SiInputs { get; set; }

        [JsonPropertyName("siTargetSystem")]
        public string? SiTargetSystem { get; set; }

        [JsonPropertyName("siChangeRecord")]
        public string? SiChangeRecord { get; set; }

        [JsonPropertyName("siJobStatement")]
        public string? SiJobStatement { get; set; }

        [JsonPropertyName("siRunAfter")]
        public string? SiRunAfter { get; set; }

        [JsonPropertyName("siExpires")]
        public string? SiExpires { get; set; }
    }

    public class ServiceActionRequest
    {
        [JsonPropertyName("siComment")]
        public string? SiComment { get; set; }
    }

    public class JobStatementResponse
    {
        [JsonPropertyName("lockHolder")]
        public string? LockHolder { get; set; }

        [JsonPropertyName("jobStatements")]
        public List<JobStatementItem>? JobStatements { get; set; }
    }

    public class JobStatementItem
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("jobStatement")]
        public string? JobStatement { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public class TargetSystemResponse
    {
        [JsonPropertyName("lockHolder")]
        public string? LockHolder { get; set; }

        [JsonPropertyName("targetSystems")]
        public List<TargetSystemItem>? TargetSystems { get; set; }
    }

    public class TargetSystemItem
    {
        [JsonPropertyName("systemNickName")]
        public string? SystemNickName { get; set; }

        [JsonPropertyName("systemName")]
        public string? SystemName { get; set; }

        [JsonPropertyName("enabled")]
        public bool? Enabled { get; set; }
    }
}
