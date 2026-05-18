using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface IJobService
    {
        Task<string> GetJobStatusAsync(string jobName, string jobId, CancellationToken cancellationToken = default);
        Task<string> SubmitJobAndWaitAsync(string datasetPath, CancellationToken cancellationToken = default);
    }
}
