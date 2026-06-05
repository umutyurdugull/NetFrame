using NetFrame.Models;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface ISystemService
    {
        Task<ZosmfInfoResponse> GetInfoAsync(CancellationToken cancellationToken = default);
    }
}
