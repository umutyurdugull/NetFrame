using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetFrame.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class JobService : IJobService
    {
        private static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly ILogger<JobService> _logger;
        private readonly ZosmfConfig _config;
        private readonly IDatasetService _datasetService;

        public JobService(
            HttpClient httpClient, 
            ILogger<JobService> logger, 
            IOptions<ZosmfConfig> config,
            IDatasetService datasetService)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
            _datasetService = datasetService ?? throw new ArgumentNullException(nameof(datasetService));
        }

        public async Task<string> GetJobStatusAsync(string jobName, string jobId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(jobName)) throw new ArgumentException("Job name cannot be empty.", nameof(jobName));
            if (string.IsNullOrWhiteSpace(jobId)) throw new ArgumentException("Job ID cannot be empty.", nameof(jobId));

            var endpoint = $"/zosmf/restjobs/jobs/{Uri.EscapeDataString(jobName)}/{Uri.EscapeDataString(jobId)}";
            using var response = await _httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<string> SubmitJobAndWaitAsync(JobSubmissionOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            string jobResponse;
            HttpRequestMessage? request = null;
            try
            {
                if (!string.IsNullOrEmpty(options.LocalFilePath))
                {
                    if (string.IsNullOrEmpty(options.DestinationDataset))
                        throw new ArgumentException("Destination dataset is required for local file upload.");

                    var fileContent = await File.ReadAllTextAsync(options.LocalFilePath, cancellationToken).ConfigureAwait(false);
                    await _datasetService.WriteDatasetContentAsync(
                        options.DestinationDataset,
                        options.DestinationMember,
                        fileContent,
                        cancellationToken: cancellationToken).ConfigureAwait(false);

                    var memberSuffix = !string.IsNullOrEmpty(options.DestinationMember) ? $"({options.DestinationMember})" : "";
                    options.DatasetPath = $"//'{options.DestinationDataset}{memberSuffix}'";
                }

                if (!string.IsNullOrEmpty(options.DatasetPath))
                {
                    request = new HttpRequestMessage(HttpMethod.Put, "/zosmf/restjobs/jobs");
                    var requestBody = new { file = options.DatasetPath };
                    request.Content = JsonContent.Create(requestBody);
                }
                else if (!string.IsNullOrEmpty(options.JclContent))
                {
                    request = new HttpRequestMessage(HttpMethod.Put, "/zosmf/restjobs/jobs");
                    request.Content = new StringContent(options.JclContent, Encoding.UTF8, "text/plain");
                    if (!string.IsNullOrEmpty(options.IntrdrMode))
                    {
                        request.Headers.Add("X-IBM-Intrdr-Mode", options.IntrdrMode);
                    }
                }
                else
                {
                    throw new ArgumentException("Either DatasetPath, JclContent, or LocalFilePath must be provided.");
                }

                using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    jobResponse = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                request?.Dispose();
            }

            if (string.IsNullOrEmpty(jobResponse))
            {
                throw new InvalidOperationException("Empty response received from job submission.");
            }

            return await PollJobStatusAsync(jobResponse, cancellationToken).ConfigureAwait(false);
        }

        private async Task<string> PollJobStatusAsync(string initialResponse, CancellationToken cancellationToken)
        {
            var job = JsonSerializer.Deserialize<ZosJob>(initialResponse, DefaultJsonOptions);
            
            string? jobName = job?.JobName;
            string? jobId = job?.JobId;
            string? currentStatus = job?.Status;

            if (string.IsNullOrEmpty(jobName) || string.IsNullOrEmpty(jobId))
            {
                throw new InvalidOperationException("Job name or ID not found in submission response.");
            }

            string jobResponse = initialResponse;
            int attempt = 0;
            var random = new Random();

            while (currentStatus != "OUTPUT" && attempt < _config.MaxPollingAttempts)
            {
                double factor = Math.Pow(_config.PollingBackoffFactor, attempt);
                double baseDelayMs = _config.PollingIntervalSeconds * 1000.0 * factor;
                
                int jitterMs = random.Next(0, 1000);
                int totalDelayMs = (int)Math.Min(baseDelayMs + jitterMs, 30000);

                _logger.LogInformation("Job {JobName} ({JobId}) status: {Status}. Polling attempt {Attempt}/{Max}. Waiting {Delay}ms.", 
                    jobName, jobId, currentStatus, attempt + 1, _config.MaxPollingAttempts, totalDelayMs);

                await Task.Delay(totalDelayMs, cancellationToken).ConfigureAwait(false);

                string statusResponse = await GetJobStatusAsync(jobName, jobId, cancellationToken).ConfigureAwait(false);
                var updatedJob = JsonSerializer.Deserialize<ZosJob>(statusResponse, DefaultJsonOptions);
                currentStatus = updatedJob?.Status;
                jobResponse = statusResponse;

                attempt++;
            }

            if (currentStatus != "OUTPUT")
            {
                _logger.LogWarning("Job {JobName} did not reach OUTPUT status within the allocated time.", jobName);
            }

            return jobResponse;
        }

        public async Task<List<ZosJob>> ListJobsAsync(
            string? owner = null, 
            string? prefix = null, 
            string? jobId = null, 
            string? maxJobs = null, 
            string? execData = null, 
            string? status = null, 
            CancellationToken cancellationToken = default)
        {
            var queryParams = new List<string>(6);
            if (!string.IsNullOrWhiteSpace(owner)) queryParams.Add($"owner={Uri.EscapeDataString(owner)}");
            if (!string.IsNullOrWhiteSpace(prefix)) queryParams.Add($"prefix={Uri.EscapeDataString(prefix)}");
            if (!string.IsNullOrWhiteSpace(jobId)) queryParams.Add($"jobid={Uri.EscapeDataString(jobId)}");
            if (!string.IsNullOrWhiteSpace(maxJobs)) queryParams.Add($"max-jobs={Uri.EscapeDataString(maxJobs)}");
            if (!string.IsNullOrWhiteSpace(execData)) queryParams.Add($"exec-data={Uri.EscapeDataString(execData)}");
            if (!string.IsNullOrWhiteSpace(status)) queryParams.Add($"status={Uri.EscapeDataString(status)}");

            var endpoint = "/zosmf/restjobs/jobs";
            if (queryParams.Count > 0)
            {
                endpoint += "?" + string.Join("&", queryParams);
            }

            using var response = await _httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ZosJob>>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new List<ZosJob>();
        }

        public async Task<List<ZosJobFile>> ListJobFilesAsync(string jobName, string jobId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(jobName)) throw new ArgumentException("Job name cannot be empty.", nameof(jobName));
            if (string.IsNullOrWhiteSpace(jobId)) throw new ArgumentException("Job ID cannot be empty.", nameof(jobId));

            var endpoint = $"/zosmf/restjobs/jobs/{Uri.EscapeDataString(jobName)}/{Uri.EscapeDataString(jobId)}/files";
            using var response = await _httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ZosJobFile>>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new List<ZosJobFile>();
        }

        public async Task<string> GetJobFileRecordsAsync(string jobName, string jobId, string fileId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(jobName)) throw new ArgumentException("Job name cannot be empty.", nameof(jobName));
            if (string.IsNullOrWhiteSpace(jobId)) throw new ArgumentException("Job ID cannot be empty.", nameof(jobId));
            if (string.IsNullOrWhiteSpace(fileId)) throw new ArgumentException("File ID cannot be empty.", nameof(fileId));

            var endpoint = $"/zosmf/restjobs/jobs/{Uri.EscapeDataString(jobName)}/{Uri.EscapeDataString(jobId)}/files/{Uri.EscapeDataString(fileId)}/records";
            using var response = await _httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task<List<ZosJobFile>> GetJobSpoolFilesAsync(string jobName, string jobId, CancellationToken cancellationToken = default)
        {
            return ListJobFilesAsync(jobName, jobId, cancellationToken);
        }

        public Task<string> GetSpoolFileContentAsync(string jobName, string jobId, string fileId, CancellationToken cancellationToken = default)
        {
            return GetJobFileRecordsAsync(jobName, jobId, fileId, cancellationToken);
        }

        public Task<JobFeedback> PurgeJobAsync(string jobName, string jobId, string version = "2.0", CancellationToken cancellationToken = default)
        {
            return DeleteJobAsync(jobName, jobId, version, cancellationToken);
        }

        public async Task<JobFeedback> HoldJobAsync(string jobName, string jobId, string version = "2.0", CancellationToken cancellationToken = default)
        {
            var endpoint = $"/zosmf/restjobs/jobs/{Uri.EscapeDataString(jobName)}/{Uri.EscapeDataString(jobId)}";
            var requestBody = new { request = "hold", version };
            return await PutJobActionAsync(endpoint, requestBody, cancellationToken).ConfigureAwait(false);
        }

        public async Task<JobFeedback> ReleaseJobAsync(string jobName, string jobId, string version = "2.0", CancellationToken cancellationToken = default)
        {
            var endpoint = $"/zosmf/restjobs/jobs/{Uri.EscapeDataString(jobName)}/{Uri.EscapeDataString(jobId)}";
            var requestBody = new { request = "release", version };
            return await PutJobActionAsync(endpoint, requestBody, cancellationToken).ConfigureAwait(false);
        }

        public async Task<JobFeedback> ChangeJobClassAsync(string jobName, string jobId, string newJobClass, string version = "2.0", CancellationToken cancellationToken = default)
        {
            var endpoint = $"/zosmf/restjobs/jobs/{Uri.EscapeDataString(jobName)}/{Uri.EscapeDataString(jobId)}";
            var requestBody = new { @class = newJobClass, version };
            return await PutJobActionAsync(endpoint, requestBody, cancellationToken).ConfigureAwait(false);
        }

        public async Task<JobFeedback> CancelJobAsync(string jobName, string jobId, string version = "2.0", CancellationToken cancellationToken = default)
        {
            var endpoint = $"/zosmf/restjobs/jobs/{Uri.EscapeDataString(jobName)}/{Uri.EscapeDataString(jobId)}";
            var requestBody = new { request = "cancel", version };
            return await PutJobActionAsync(endpoint, requestBody, cancellationToken).ConfigureAwait(false);
        }

        public async Task<JobFeedback> DeleteJobAsync(string jobName, string jobId, string version = "2.0", CancellationToken cancellationToken = default)
        {
            var endpoint = $"/zosmf/restjobs/jobs/{Uri.EscapeDataString(jobName)}/{Uri.EscapeDataString(jobId)}";
            using var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            request.Headers.Add("X-IBM-Job-Modify-Version", version);

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                return new JobFeedback { Status = "202", Message = "Accepted" };
            }
            return await response.Content.ReadFromJsonAsync<JobFeedback>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new JobFeedback();
        }

        private async Task<JobFeedback> PutJobActionAsync(string endpoint, object requestBody, CancellationToken cancellationToken)
        {
            using var response = await _httpClient.PutAsJsonAsync(endpoint, requestBody, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                return new JobFeedback { Status = "202", Message = "Accepted" };
            }
            return await response.Content.ReadFromJsonAsync<JobFeedback>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new JobFeedback();
        }
    }
}
