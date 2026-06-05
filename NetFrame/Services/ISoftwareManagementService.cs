using NetFrame.Models.SoftwareManagement;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface ISoftwareManagementService
    {
        Task<List<SoftwareInstanceSummary>> ListSoftwareInstancesAsync(CancellationToken cancellationToken = default);
        
        Task<SoftwareInstanceDetail> GetSoftwareInstanceAsync(string systemNickname, string swiName, CancellationToken cancellationToken = default);
        Task<SoftwareInstanceDetail> GetSoftwareInstanceByUuidAsync(string uuid, CancellationToken cancellationToken = default);

        Task AddSoftwareInstanceAsync(SoftwareInstanceRequest request, CancellationToken cancellationToken = default);
        Task ModifySoftwareInstanceAsync(string uuid, SoftwareInstanceRequest request, CancellationToken cancellationToken = default);
        Task DeleteSoftwareInstanceAsync(string systemNickname, string swiName, CancellationToken cancellationToken = default);

        // Async Operations
        Task<string> StartListDataSetsAsync(string uuid, CancellationToken cancellationToken = default);
        Task<DatasetListStatusResponse> GetListDataSetsStatusAsync(string statusUrl, CancellationToken cancellationToken = default);

        Task<string> StartExportAsync(string uuid, ExportRequest request, CancellationToken cancellationToken = default);
        Task<ExportStatusResponse> GetExportStatusAsync(string statusUrl, CancellationToken cancellationToken = default);
    }
}
