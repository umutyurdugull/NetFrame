using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface IRmfMeteringService
    {
        Task<string> GetMeterDataAsync(CancellationToken cancellationToken = default);
    }
}
