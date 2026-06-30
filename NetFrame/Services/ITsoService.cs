using System.Threading;
using System.Threading.Tasks;
using NetFrame.Models;

namespace NetFrame.Services
{
    public interface ITsoService
    {
        Task<TsoCommandResponse> ExecuteTsoCommandAsync(string command, string? system = null, int? maxWaitTime = null, CancellationToken cancellationToken = default);
    }
}
