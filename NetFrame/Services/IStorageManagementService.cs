using NetFrame.Models.StorageManagement;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface IStorageManagementService
    {
        // Storage Group Operations
        Task<List<StorageGroupSummary>> ListStorageGroupsAsync(string? filter = null, string? type = null, bool detailData = false, CancellationToken cancellationToken = default);
        Task<StorageGroupDetail> GetStorageGroupDefinitionAsync(string scdsName, string stgName, bool detailData = false, CancellationToken cancellationToken = default);

        // Volume Operations
        Task<List<VolumeSummary>> ListVolumesAsync(string? filter = null, string? stgName = null, int offset = 0, int limit = 0, CancellationToken cancellationToken = default);
        Task<VolumeSummary> GetVolumeDefinitionAsync(string volumeSer, CancellationToken cancellationToken = default);

        // Data Class Operations
        Task<List<DataClassSummary>> ListDataClassesAsync(string? filter = null, bool detailData = false, int offset = 0, int limit = 0, CancellationToken cancellationToken = default);
    }
}
