using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetFrame.Models.SoftwareManagement
{
    // --- List Response ---
    public class SoftwareInstanceListResponse
    {
        [JsonPropertyName("swilist")]
        public List<SoftwareInstanceSummary>? SwiList { get; set; }
    }

    public class SoftwareInstanceSummary
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("system")]
        public string System { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("uuid")]
        public string? Uuid { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("swiurl")]
        public string? SwiUrl { get; set; }
    }

    // --- Detailed Properties & Request ---
    public class SoftwareInstanceRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("system")]
        public string System { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("globalzone")]
        public string? GlobalZone { get; set; }

        [JsonPropertyName("targetzones")]
        public List<string>? TargetZones { get; set; }

        [JsonPropertyName("categories")]
        public List<string>? Categories { get; set; }

        [JsonPropertyName("datasets")]
        public List<SwiDataset>? Datasets { get; set; }

        [JsonPropertyName("products")]
        public List<NonSmpeProduct>? Products { get; set; }
    }

    public class SoftwareInstanceDetail : SoftwareInstanceRequest
    {
        [JsonPropertyName("uuid")]
        public string? Uuid { get; set; }

        [JsonPropertyName("productinforetrieved")]
        public string? ProductInfoRetrieved { get; set; }

        [JsonPropertyName("lastmodified")]
        public string? LastModified { get; set; }

        [JsonPropertyName("created")]
        public string? Created { get; set; }

        [JsonPropertyName("locked")]
        public string? Locked { get; set; }
    }

    public class SwiDataset
    {
        [JsonPropertyName("dsname")]
        public string DsName { get; set; } = string.Empty;

        [JsonPropertyName("volume")]
        public string? Volume { get; set; }
    }

    public class NonSmpeProduct
    {
        [JsonPropertyName("prodname")]
        public string? ProdName { get; set; }

        [JsonPropertyName("prodid")]
        public string? ProdId { get; set; }

        [JsonPropertyName("release")]
        public string? Release { get; set; }

        [JsonPropertyName("vendor")]
        public string? Vendor { get; set; }
    }

    // --- Async Operations ---
    public class AsyncStatusResponse
    {
        [JsonPropertyName("statusurl")]
        public string? StatusUrl { get; set; }
    }

    public class SoftwareInstanceTaskStatus
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty; // running, complete

        [JsonPropertyName("percentcomplete")]
        public string? PercentComplete { get; set; }
    }

    public class DatasetListStatusResponse : SoftwareInstanceTaskStatus
    {
        [JsonPropertyName("swidatasets")]
        public SoftwareInstanceDatasets? SwiDatasets { get; set; }
    }

    public class SoftwareInstanceDatasets
    {
        [JsonPropertyName("smpemanageddatasets")]
        public List<ManagedDataset>? SmpeManagedDatasets { get; set; }

        [JsonPropertyName("nonsmpemanageddatasets")]
        public List<ManagedDataset>? NonSmpeManagedDatasets { get; set; }
    }

    public class ManagedDataset
    {
        [JsonPropertyName("dsname")]
        public string DsName { get; set; } = string.Empty;

        [JsonPropertyName("volumes")]
        public List<string>? Volumes { get; set; }

        [JsonPropertyName("dstype")]
        public string? DsType { get; set; }
    }

    // --- Export ---
    public class ExportRequest
    {
        [JsonPropertyName("packagedir")]
        public string PackageDir { get; set; } = string.Empty;

        [JsonPropertyName("jcldataset")]
        public string JclDataset { get; set; } = string.Empty;

        [JsonPropertyName("includedlibs")]
        public string? IncludedLibs { get; set; } // yes or no

        [JsonPropertyName("jobstatement")]
        public List<string>? JobStatement { get; set; }
    }

    public class ExportStatusResponse : SoftwareInstanceTaskStatus
    {
        [JsonPropertyName("jcl")]
        public List<string>? Jcl { get; set; }
    }
}
