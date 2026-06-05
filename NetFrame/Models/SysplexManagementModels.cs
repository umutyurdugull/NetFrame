using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetFrame.Models.SysplexManagement
{
    // --- List Response ---
    public class CfrmPolicyListResponse
    {
        [JsonPropertyName("items")]
        public List<CfrmPolicySummary>? Items { get; set; }

        [JsonPropertyName("activePolicy")]
        public ActivePolicy? ActivePolicy { get; set; }

        [JsonPropertyName("_json_version")]
        public string? JsonVersion { get; set; }
    }

    public class CfrmPolicySummary
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("_user")]
        public string? User { get; set; }

        [JsonPropertyName("_defined")]
        public string? Defined { get; set; }
    }

    public class ActivePolicy
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("activatedDate")]
        public string? ActivatedDate { get; set; }
    }

    // --- Detail Response ---
    public class CfrmPolicyDetailResponse
    {
        [JsonPropertyName("data type")]
        public string? DataType { get; set; }

        [JsonPropertyName("_version_supported")]
        public string? VersionSupported { get; set; }

        [JsonPropertyName("policy")]
        public List<CfrmPolicyDetail>? Policies { get; set; }
    }

    public class CfrmPolicyDetail
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("_defined")]
        public string? Defined { get; set; }

        [JsonPropertyName("_user")]
        public string? User { get; set; }

        [JsonPropertyName("_version")]
        public string? Version { get; set; }

        [JsonPropertyName("cf")]
        public List<CouplingFacility>? CouplingFacilities { get; set; }

        [JsonPropertyName("structure")]
        public List<CfStructure>? Structures { get; set; }
    }

    public class CouplingFacility
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("mfg")]
        public string? Mfg { get; set; }

        [JsonPropertyName("plant")]
        public string? Plant { get; set; }

        [JsonPropertyName("sequence")]
        public string? Sequence { get; set; }

        [JsonPropertyName("partition")]
        public string? Partition { get; set; }

        [JsonPropertyName("side")]
        public string? Side { get; set; }

        [JsonPropertyName("cpcid")]
        public string? Cpcid { get; set; }

        [JsonPropertyName("dumpspace")]
        public long? DumpSpace { get; set; }

        [JsonPropertyName("site")]
        public string? Site { get; set; }
    }

    public class CfStructure
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("size")]
        public long? Size { get; set; }

        [JsonPropertyName("initsize")]
        public long? InitSize { get; set; }

        [JsonPropertyName("minsize")]
        public long? MinSize { get; set; }

        [JsonPropertyName("scmmaxsize")]
        public long? ScmMaxSize { get; set; }

        [JsonPropertyName("fullthreshold")]
        public int? FullThreshold { get; set; }

        [JsonPropertyName("preflist")]
        public List<string>? PrefList { get; set; }

        [JsonPropertyName("excllist")]
        public List<string>? ExclList { get; set; }

        [JsonPropertyName("rebuildpercent")]
        public int? RebuildPercent { get; set; }

        [JsonPropertyName("duplex")]
        public string? Duplex { get; set; }

        [JsonPropertyName("recprty")]
        public int? RecPrty { get; set; }
    }
}
