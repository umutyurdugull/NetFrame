using NetFrame.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface IDatasetService
    {
        Task<List<string>> ListDatasetsAsync(string dsLevel, CancellationToken cancellationToken = default);

        Task<DatasetMemberResponse> ListDatasetMembersAsync(
            string datasetName,
            ListMembersOptions? options = null,
            CancellationToken cancellationToken = default);

        Task<string?> RetrieveDatasetContentAsync(
            string datasetName,
            string? memberName = null,
            RetrieveContentOptions? options = null,
            CancellationToken cancellationToken = default);

        Task<string> WriteDatasetContentAsync(
            string datasetName,
            string? memberName = null,
            string content = "",
            string? volser = null,
            WriteContentOptions? options = null,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteDatasetAsync(
            string datasetName,
            string? memberName = null,
            string? volser = null,
            DeleteDatasetOptions? options = null,
            CancellationToken cancellationToken = default);

        Task CreateDatasetAsync(
            string datasetName,
            CreateDatasetRequest requestBody,
            CreateDatasetOptions? options = null,
            CancellationToken cancellationToken = default);

        Task<DatasetAttributesResponse?> GetDatasetAttributesAsync(
            string datasetName,
            CancellationToken cancellationToken = default);

        Task<bool> CreateMemberAsync(
            string datasetName,
            string memberName,
            string content = "",
            CancellationToken cancellationToken = default);

        Task<bool> RecallDatasetAsync(
            string datasetName,
            MigratedRecallMode mode = MigratedRecallMode.Wait,
            CancellationToken cancellationToken = default);

        Task<string?> DownloadDatasetChunkedAsync(
            string datasetName,
            string? memberName = null,
            int startRecord = 0,
            int recordCount = 1000,
            CancellationToken cancellationToken = default);

        Task<bool> CopyDatasetAsync(
            string sourceDatasetName,
            string targetDatasetName,
            string? sourceMemberName = null,
            string? targetMemberName = null,
            bool replace = true,
            CancellationToken cancellationToken = default);

        Task<bool> CreateVsamClusterAsync(
            string vsamName,
            CreateVsamClusterRequest requestBody,
            CancellationToken cancellationToken = default);

        Task<bool> MigrateDatasetAsync(
            string datasetName,
            bool wait = false,
            CancellationToken cancellationToken = default);

        Task<DatasetCompareResult> CompareDatasetsAsync(
            string sourceDatasetName,
            string targetDatasetName,
            string? sourceMemberName = null,
            string? targetMemberName = null,
            CancellationToken cancellationToken = default);
    }
}
