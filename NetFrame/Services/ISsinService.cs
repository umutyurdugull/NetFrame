using NetFrame.Models.ResourceManagement;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface ISsinService
    {
        Task<SsinListResponse> CreateSsinAsync(CreateSsinRequest request, CancellationToken cancellationToken = default);
        Task<SsinListResponse> ListSsinAsync(string? name = null, string? registryId = null, CancellationToken cancellationToken = default);
        Task<VariableNameResponse> CreateVariableNameAsync(CreateVariableNameRequest request, CancellationToken cancellationToken = default);
    }
}
