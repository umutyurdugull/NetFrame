using System.Threading;
using System.Threading.Tasks;
using NetFrame.Models;

namespace NetFrame.Services
{
    public interface ITsoService
    {
        Task<TsoCommandResponse> ExecuteTsoCommandAsync(string command, string? system = null, int? maxWaitTime = null, CancellationToken cancellationToken = default);

        Task<TsoCommandResponse> StartInteractiveTsoSessionAsync(string? logonProcedure = null, CancellationToken cancellationToken = default);
        Task<TsoCommandResponse> SendTsoSessionInputAsync(string servletKey, string commandData, CancellationToken cancellationToken = default);
        Task EndTsoSessionAsync(string servletKey, CancellationToken cancellationToken = default);
    }
}
