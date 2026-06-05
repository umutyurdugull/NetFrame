using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetFrame.Models.StorageManagement
{
    // --- Storage Group Models ---
    public class StorageGroupSummary
    {
        [JsonPropertyName("storageGroupName")]
        public string StorageGroupName { get; set; } = string.Empty;

        [JsonPropertyName("storageGroupType")]
        public string? StorageGroupType { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("numberOfVolumes")]
        public int? NumberOfVolumes { get; set; }

        [JsonPropertyName("spaceAvailable")]
        public double? SpaceAvailable { get; set; } // GB

        [JsonPropertyName("spaceUsed")]
        public double? SpaceUsed { get; set; } // GB

        [JsonPropertyName("totalSpace")]
        public double? TotalSpace { get; set; } // GB

        [JsonPropertyName("lastUser")]
        public string? LastUser { get; set; }

        [JsonPropertyName("updateDate")]
        public string? UpdateDate { get; set; }

        [JsonPropertyName("updateTime")]
        public string? UpdateTime { get; set; }

        [JsonPropertyName("status")]
        public List<StorageGroupStatus>? Status { get; set; }
    }

    public class StorageGroupStatus
    {
        [JsonPropertyName("sysName")]
        public string? SysName { get; set; }

        [JsonPropertyName("sysType")]
        public string? SysType { get; set; } // 1: system, 2: sysplex

        [JsonPropertyName("requestedSystemStatus")]
        public string? RequestedSystemStatus { get; set; }

        [JsonPropertyName("confirmedSmsStatus")]
        public string? ConfirmedSmsStatus { get; set; }
    }

    public class StorageGroupDetail : StorageGroupSummary
    {
        [JsonPropertyName("autoBackupSystem")]
        public string? AutoBackupSystem { get; set; }

        [JsonPropertyName("autoDump")]
        public bool? AutoDump { get; set; }

        [JsonPropertyName("autoMigration")]
        public bool? AutoMigration { get; set; }

        [JsonPropertyName("highThreshold")]
        public int? HighThreshold { get; set; }

        [JsonPropertyName("lowThreshold")]
        public int? LowThreshold { get; set; }

        // ... many more detailed fields could be added here as needed
    }

    // --- Volume Models ---
    public class VolumeSummary
    {
        [JsonPropertyName("volumeSerial")]
        public string VolumeSerial { get; set; } = string.Empty;

        [JsonPropertyName("storageGroupName")]
        public string? StorageGroupName { get; set; }

        [JsonPropertyName("storageGroupStatus")]
        public string? StorageGroupStatus { get; set; }

        [JsonPropertyName("freeSpace")]
        public double? FreeSpace { get; set; } // MB

        [JsonPropertyName("totalCapacity")]
        public double? TotalCapacity { get; set; } // MB

        [JsonPropertyName("fullVolumeLastUsed")]
        public int? FullVolumeLastUsed { get; set; } // %

        [JsonPropertyName("lastUser")]
        public string? LastUser { get; set; }

        [JsonPropertyName("updateDate")]
        public string? UpdateDate { get; set; }

        [JsonPropertyName("updateTime")]
        public string? UpdateTime { get; set; }

        [JsonPropertyName("status")]
        public List<VolumeStatus>? Status { get; set; }
    }

    public class VolumeStatus
    {
        [JsonPropertyName("sysName")]
        public string? SysName { get; set; }

        [JsonPropertyName("sysType")]
        public string? SysType { get; set; }

        [JsonPropertyName("mvsSystemStatus")]
        public string? MvsSystemStatus { get; set; }

        [JsonPropertyName("requestedSystemStatus")]
        public string? RequestedSystemStatus { get; set; }

        [JsonPropertyName("confirmedSmsStatus")]
        public string? ConfirmedSmsStatus { get; set; }
    }

    // --- Data Class Models ---
    public class DataClassSummary
    {
        [JsonPropertyName("dataClassName")]
        public string DataClassName { get; set; } = string.Empty;

        [JsonPropertyName("recorg")]
        public string? Recorg { get; set; }

        [JsonPropertyName("recfm")]
        public string? Recfm { get; set; }

        [JsonPropertyName("dsnType")]
        public string? DsnType { get; set; }

        [JsonPropertyName("recordLength")]
        public string? RecordLength { get; set; }

        [JsonPropertyName("lastUser")]
        public string? LastUser { get; set; }

        [JsonPropertyName("updateDate")]
        public string? UpdateDate { get; set; }

        [JsonPropertyName("updateTime")]
        public string? UpdateTime { get; set; }
    }
}
