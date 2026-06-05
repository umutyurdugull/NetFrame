using NetFrame.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface IJobService
    {
        Task<string> GetJobStatusAsync(string jobName, string jobId, CancellationToken cancellationToken = default);
        Task<string> SubmitJobAndWaitAsync(JobSubmissionOptions options, CancellationToken cancellationToken = default);
        Task<List<ZosJob>> ListJobsAsync(string? owner = null, string? prefix = null, string? jobId = null, string? maxJobs = null, string? execData = null, string? status = null, CancellationToken cancellationToken = default);
        Task<List<ZosJobFile>> ListJobFilesAsync(string jobName, string jobId, CancellationToken cancellationToken = default);
        Task<string> GetJobFileRecordsAsync(string jobName, string jobId, string fileId, CancellationToken cancellationToken = default);
        
        Task<JobFeedback> HoldJobAsync(string jobName, string jobId, string version = "2.0", CancellationToken cancellationToken = default);
        Task<JobFeedback> ReleaseJobAsync(string jobName, string jobId, string version = "2.0", CancellationToken cancellationToken = default);
        Task<JobFeedback> ChangeJobClassAsync(string jobName, string jobId, string newJobClass, string version = "2.0", CancellationToken cancellationToken = default);
        Task<JobFeedback> CancelJobAsync(string jobName, string jobId, string version = "2.0", CancellationToken cancellationToken = default);
        Task<JobFeedback> DeleteJobAsync(string jobName, string jobId, string version = "2.0", CancellationToken cancellationToken = default);
    }
}
