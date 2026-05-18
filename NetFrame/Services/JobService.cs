using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetFrame.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
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

        public JobService(HttpClient httpClient, ILogger<JobService> logger, IOptions<ZosmfConfig> config)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
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

        public async Task<string> SubmitJobAndWaitAsync(string datasetPath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(datasetPath)) throw new ArgumentException("Dataset path cannot be empty.", nameof(datasetPath));

            var endpoint = "/zosmf/restjobs/jobs";
            var requestBody = new { file = datasetPath };

            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, requestBody, cancellationToken);
                response.EnsureSuccessStatusCode();

                var jobResponse = await response.Content.ReadAsStringAsync(cancellationToken);
                if (string.IsNullOrEmpty(jobResponse))
                {
                    throw new InvalidOperationException("Empty response received from job submission.");
                }

                var job = JsonNode.Parse(jobResponse);
                string? jobName = job?["jobname"]?.ToString();
                string? jobId = job?["jobid"]?.ToString();
                string? currentStatus = job?["status"]?.ToString();

                if (string.IsNullOrEmpty(jobName) || string.IsNullOrEmpty(jobId))
                {
                    throw new InvalidOperationException("Job name or ID not found in submission response.");
                }

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
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Job submission/wait was cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting job for dataset: {DatasetPath}", datasetPath);
                throw;
            }
        }
    }
}
