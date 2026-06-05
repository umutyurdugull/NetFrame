using NetFrame.Models.WlmResourcePooling;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface IWlmResourcePoolingService
    {
        Task<PrimeWrpResponse> PrimeWlmResourcePoolAsync(PrimeWrpRequest request, CancellationToken cancellationToken = default);
        Task<DeleteWrpResponse> DeleteWlmResourcePoolAsync(string wrpId, CancellationToken cancellationToken = default);
        Task<ConstructPolicyResponse> ConstructWlmServiceDefinitionAsync(ConstructPolicyRequest request, CancellationToken cancellationToken = default);
    }
}
