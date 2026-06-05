using NetFrame.Models.Workflow;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface IWorkflowService
    {
        Task<CreateWorkflowResponse> CreateWorkflowAsync(CreateWorkflowRequest request, CancellationToken cancellationToken = default);
        Task<WorkflowProperties> GetWorkflowPropertiesAsync(string workflowKey, string? returnData = null, CancellationToken cancellationToken = default);
        Task<ListWorkflowsResponse> ListWorkflowsAsync(string? workflowName = null, string? system = null, string? owner = null, CancellationToken cancellationToken = default);
        Task StartWorkflowAsync(string workflowKey, StartWorkflowRequest? request = null, CancellationToken cancellationToken = default);
        Task<CancelWorkflowResponse> CancelWorkflowAsync(string workflowKey, CancellationToken cancellationToken = default);
        Task DeleteWorkflowAsync(string workflowKey, CancellationToken cancellationToken = default);
        Task<string> GetWorkflowDefinitionAsync(string definitionFilePath, string? system = null, string? returnData = null, CancellationToken cancellationToken = default);
        Task<ArchiveWorkflowResponse> ArchiveWorkflowAsync(string workflowKey, CancellationToken cancellationToken = default);
        Task<ListArchivedWorkflowsResponse> ListArchivedWorkflowsAsync(string? orderBy = null, string? view = null, CancellationToken cancellationToken = default);
        Task<WorkflowProperties> GetArchivedWorkflowPropertiesAsync(string workflowKey, string? returnData = null, CancellationToken cancellationToken = default);
        Task DeleteArchivedWorkflowAsync(string workflowKey, CancellationToken cancellationToken = default);
    }
}
