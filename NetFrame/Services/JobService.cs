using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetFrame.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class JobService : IJobService
    {
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
            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting job status for {JobName} ({JobId})", jobName, jobId);
                throw;
            }
        }

        public async Task<string> SubmitJobAndWaitAsync(JobSubmissionOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            HttpRequestMessage? request = null;

            try
            {
                
                if (!string.IsNullOrEmpty(options.LocalFilePath))
                {
                    if (string.IsNullOrEmpty(options.DestinationDataset))
                        throw new ArgumentException("Destination dataset is required for local file upload.");

                    var fileContent = await File.ReadAllTextAsync(options.LocalFilePath, cancellationToken);
                    await _datasetService.WriteDatasetContentAsync(
                        options.DestinationDataset,
                        options.DestinationMember,
                        fileContent,
                        cancellationToken: cancellationToken);

                    // Formatting as "//'DATASET(MEMBER)'" as required by z/OSMF
                    var memberSuffix = !string.IsNullOrEmpty(options.DestinationMember) ? $"({options.DestinationMember})" : "";
                    options.DatasetPath = $"//'{options.DestinationDataset}{memberSuffix}'";
                }

                // jcl mainframe
                if (!string.IsNullOrEmpty(options.DatasetPath))
                {
                    request = new HttpRequestMessage(HttpMethod.Put, "/zosmf/restjobs/jobs");
                    var requestBody = new { file = options.DatasetPath };
                    request.Content = JsonContent.Create(requestBody);
                }
                // jcl kodun icinde 
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

                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("z/OSMF API Error: {StatusCode} - {ErrorDetails}", response.StatusCode, errorContent);
                    response.EnsureSuccessStatusCode();
                }

                var jobResponse = await response.Content.ReadAsStringAsync(cancellationToken);
                if (string.IsNullOrEmpty(jobResponse))
                {
                    throw new InvalidOperationException("Empty response received from job submission.");
                }

                return await PollJobStatusAsync(jobResponse, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting job.");
                throw;
            }
            finally
            {
                request?.Dispose();
            }
        }

        private async Task<string> PollJobStatusAsync(string initialResponse, CancellationToken cancellationToken)
        {
            var job = JsonNode.Parse(initialResponse);
            string? jobName = job?["jobname"]?.ToString();
            string? jobId = job?["jobid"]?.ToString();
            string? currentStatus = job?["status"]?.ToString();

            if (string.IsNullOrEmpty(jobName) || string.IsNullOrEmpty(jobId))
            {
                throw new InvalidOperationException("Job name or ID not found in submission response.");
            }

            string jobResponse = initialResponse;
            int attempt = 0;
            while (currentStatus != "OUTPUT" && attempt < _config.MaxPollingAttempts)
            {
                _logger.LogInformation("Job {JobName} status: {Status}. Polling attempt {Attempt}/{Max}", jobName, currentStatus, attempt + 1, _config.MaxPollingAttempts);

                await Task.Delay(TimeSpan.FromSeconds(_config.PollingIntervalSeconds), cancellationToken);

                string statusResponse = await GetJobStatusAsync(jobName, jobId, cancellationToken);
                var updatedJob = JsonNode.Parse(statusResponse);
                currentStatus = updatedJob?["status"]?.ToString();
                jobResponse = statusResponse;

                attempt++;
            }

            if (currentStatus != "OUTPUT")
            {
                _logger.LogWarning("Job {JobName} did not reach OUTPUT status within the allocated time.", jobName);
            }

            return jobResponse;
        }

        public async Task<System.Collections.Generic.List<ZosJob>> ListJobsAsync(string? owner = null, string? prefix = null, string? jobId = null, string? maxJobs = null, string? execData = null, string? status = null, CancellationToken cancellationToken = default)
        {
            var queryParams = new System.Collections.Generic.List<string>();
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

            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<System.Collections.Generic.List<ZosJob>>(cancellationToken: cancellationToken) ?? new System.Collections.Generic.List<ZosJob>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing jobs");
                throw;
            }
        }

        public async Task<System.Collections.Generic.List<ZosJobFile>> ListJobFilesAsync(string jobName, string jobId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(jobName)) throw new ArgumentException("Job name cannot be empty.", nameof(jobName));
            if (string.IsNullOrWhiteSpace(jobId)) throw new ArgumentException("Job ID cannot be empty.", nameof(jobId));

            var endpoint = $"/zosmf/restjobs/jobs/{Uri.EscapeDataString(jobName)}/{Uri.EscapeDataString(jobId)}/files";
            
            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<System.Collections.Generic.List<ZosJobFile>>(cancellationToken: cancellationToken) ?? new System.Collections.Generic.List<ZosJobFile>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing files for job {JobName} ({JobId})", jobName, jobId);
                throw;
            }
        }

        public async Task<string> GetJobFileRecordsAsync(string jobName, string jobId, string fileId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(jobName)) throw new ArgumentException("Job name cannot be empty.", nameof(jobName));
            if (string.IsNullOrWhiteSpace(jobId)) throw new ArgumentException("Job ID cannot be empty.", nameof(jobId));
            if (string.IsNullOrWhiteSpace(fileId)) throw new ArgumentException("File ID cannot be empty.", nameof(fileId));

            var endpoint = $"/zosmf/restjobs/jobs/{Uri.EscapeDataString(jobName)}/{Uri.EscapeDataString(jobId)}/files/{Uri.EscapeDataString(fileId)}/records";
            
            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading records for file {FileId} of job {JobName} ({JobId})", fileId, jobName, jobId);
                throw;
            }
        }

        public async Task<JobFeedback> HoldJobAsync(string jobName, string jobId, string version = "2.0", CancellationToken cancellationToken = default)
        {
            var endpoint = $"/zosmf/restjobs/jobs/{Uri.EscapeDataString(jobName)}/{Uri.EscapeDataString(jobId)}";
            var requestBody = new { request = "hold", version };
            return await PutJobActionAsync(endpoint, requestBody, cancellationToken);
        }

        public async Task<JobFeedback> ReleaseJobAsync(string jobName, string jobId, string version = "2.0", CancellationToken cancellationToken = default)
        {
            var endpoint = $"/zosmf/restjobs/jobs/{Uri.EscapeDataString(jobName)}/{Uri.EscapeDataString(jobId)}";
            var requestBody = new { request = "release", version };
            return await PutJobActionAsync(endpoint, requestBody, cancellationToken);
        }

        public async Task<JobFeedback> ChangeJobClassAsync(string jobName, string jobId, string newJobClass, string version = "2.0", CancellationToken cancellationToken = default)
        {
            var endpoint = $"/zosmf/restjobs/jobs/{Uri.EscapeDataString(jobName)}/{Uri.EscapeDataString(jobId)}";
            var requestBody = new { @class = newJobClass, version };
            return await PutJobActionAsync(endpoint, requestBody, cancellationToken);
        }

        public async Task<JobFeedback> CancelJobAsync(string jobName, string jobId, string version = "2.0", CancellationToken cancellationToken = default)
        {
            var endpoint = $"/zosmf/restjobs/jobs/{Uri.EscapeDataString(jobName)}/{Uri.EscapeDataString(jobId)}";
            var requestBody = new { request = "cancel", version };
            return await PutJobActionAsync(endpoint, requestBody, cancellationToken);
        }

        public async Task<JobFeedback> DeleteJobAsync(string jobName, string jobId, string version = "2.0", CancellationToken cancellationToken = default)
        {
            var endpoint = $"/zosmf/restjobs/jobs/{Uri.EscapeDataString(jobName)}/{Uri.EscapeDataString(jobId)}";
            using var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            request.Headers.Add("X-IBM-Job-Modify-Version", version);

            try
            {
                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    return new JobFeedback { Status = "202", Message = "Accepted" };
                }
                return await response.Content.ReadFromJsonAsync<JobFeedback>(cancellationToken: cancellationToken) ?? new JobFeedback();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting job {JobName} ({JobId})", jobName, jobId);
                throw;
            }
        }

        private async Task<JobFeedback> PutJobActionAsync(string endpoint, object requestBody, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, requestBody, cancellationToken);
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    return new JobFeedback { Status = "202", Message = "Accepted" };
                }
                return await response.Content.ReadFromJsonAsync<JobFeedback>(cancellationToken: cancellationToken) ?? new JobFeedback();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing job action at {Endpoint}", endpoint);
                throw;
            }
        }
    }
}
