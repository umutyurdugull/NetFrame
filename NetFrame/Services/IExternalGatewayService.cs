using NetFrame.Models;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface IExternalGatewayService
    {
        Task<ExternalGatewayResponse> GetDataAsync(ExternalGatewayRequest request, CancellationToken cancellationToken = default);
        Task<ExternalGatewayResponse> PostDataAsync(ExternalGatewayRequest request, CancellationToken cancellationToken = default);
        Task<ExternalGatewayResponse> PutDataAsync(ExternalGatewayRequest request, CancellationToken cancellationToken = default);
        Task<ExternalGatewayResponse> DeleteDataAsync(ExternalGatewayRequest request, CancellationToken cancellationToken = default);
    }
}
