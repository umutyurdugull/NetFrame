using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface IConsoleService
    {
        Task<string> IssueCommandAsync(string command, string? system = null, CancellationToken cancellationToken = default);
    }
}
