using NetFrame.Models.Provisioning;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface ISoftwareInstanceService
    {
        Task<SoftwareInstanceDetail> CreateInstanceAsync(SoftwareInstanceRequest request, CancellationToken cancellationToken = default);
        Task<SoftwareInstanceDetail> GetInstanceAsync(string objectId, CancellationToken cancellationToken = default);
        Task<InstanceListResponse> ListInstancesAsync(CancellationToken cancellationToken = default);
        Task UpdateInstanceAsync(string objectId, SoftwareInstanceRequest request, CancellationToken cancellationToken = default);
        Task DeleteInstanceAsync(string objectId, CancellationToken cancellationToken = default);

        Task<ActionResult> PerformActionAsync(string objectId, string actionName, PerformActionRequest? request = null, CancellationToken cancellationToken = default);
        Task<ActionResult> GetActionResponseAsync(string objectId, string actionId, CancellationToken cancellationToken = default);
        Task<ActionResponseList> ListActionResponsesAsync(string objectId, CancellationToken cancellationToken = default);
        
        Task ResumeProvisioningWorkflowAsync(string objectId, CancellationToken cancellationToken = default);
        Task RetryProvisioningWorkflowAsync(string objectId, CancellationToken cancellationToken = default);
        
        Task ResumeActionWorkflowAsync(string objectId, string actionId, CancellationToken cancellationToken = default);
        Task RetryActionWorkflowAsync(string objectId, string actionId, CancellationToken cancellationToken = default);

        Task UpdateVariablesAsync(string objectId, List<InstanceVariable> variables, CancellationToken cancellationToken = default);
    }
}
