using NetFrame.Models.ManagementServicesCatalog;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface IManagementServicesCatalogService
    {
        Task<List<CatalogServiceSummary>> ListCatalogServicesAsync(string? serviceName = null, string? categoryName = null, string? state = null, bool summary = true, CancellationToken cancellationToken = default);
        Task<string> GetCatalogServiceDetailsAsync(string objectId, CancellationToken cancellationToken = default);
        
        Task<List<Category>> ListCategoriesAsync(CancellationToken cancellationToken = default);
        Task<Category> GetCategoryDetailsAsync(string objectId, CancellationToken cancellationToken = default);
        
        Task<List<ServiceSubmissionSummary>> ListServiceSubmissionsAsync(string? serviceName = null, string? status = null, string? submitter = null, string? targetSystem = null, string? label = null, bool summary = true, CancellationToken cancellationToken = default);
        Task<string> GetServiceSubmissionDetailsAsync(string objectId, CancellationToken cancellationToken = default);
        
        Task<string> CreateServiceSubmissionAsync(CreateServiceSubmissionRequest request, CancellationToken cancellationToken = default);
        Task DeleteServiceSubmissionAsync(string objectId, CancellationToken cancellationToken = default);
        Task ModifyServiceSubmissionAsync(string objectId, ModifyServiceSubmissionRequest request, CancellationToken cancellationToken = default);
        Task PerformActionOnServiceSubmissionAsync(string objectId, string action, ServiceActionRequest? request = null, CancellationToken cancellationToken = default);
        
        Task<JobStatementResponse> GetJobStatementsAsync(CancellationToken cancellationToken = default);
        Task<TargetSystemResponse> GetTargetSystemsAsync(CancellationToken cancellationToken = default);
    }
}
