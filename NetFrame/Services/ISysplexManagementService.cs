using NetFrame.Models.SysplexManagement;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface ISysplexManagementService
    {
        Task<CfrmPolicyListResponse> ListCfrmPoliciesAsync(CancellationToken cancellationToken = default);
        Task<CfrmPolicyDetailResponse> GetCfrmPolicyAsync(string policyName, CancellationToken cancellationToken = default);
    }
}
