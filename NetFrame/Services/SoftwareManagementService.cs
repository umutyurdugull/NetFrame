using Microsoft.Extensions.Logging;
using NetFrame.Models.SoftwareManagement;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class SoftwareManagementService : ISoftwareManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SoftwareManagementService> _logger;
        private const string BasePath = "/zosmf/swmgmt/swi";

        public SoftwareManagementService(HttpClient httpClient, ILogger<SoftwareManagementService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<SoftwareInstanceSummary>> ListSoftwareInstancesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<SoftwareInstanceListResponse>(BasePath, cancellationToken).ConfigureAwait(false);
                return response?.SwiList ?? new List<SoftwareInstanceSummary>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing software instances.");
                throw;
            }
        }

        public async Task<SoftwareInstanceDetail> GetSoftwareInstanceAsync(string systemNickname, string swiName, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/{Uri.EscapeDataString(systemNickname)}/{Uri.EscapeDataString(swiName)}";
            return await GetInstanceInternalAsync(endpoint, cancellationToken).ConfigureAwait(false);
        }

        public async Task<SoftwareInstanceDetail> GetSoftwareInstanceByUuidAsync(string uuid, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/{Uri.EscapeDataString(uuid)}";
            return await GetInstanceInternalAsync(endpoint, cancellationToken).ConfigureAwait(false);
        }

        private async Task<SoftwareInstanceDetail> GetInstanceInternalAsync(string endpoint, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<SoftwareInstanceDetail>(endpoint, cancellationToken).ConfigureAwait(false);
                return response ?? throw new InvalidOperationException("Empty response received from get software instance endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving software instance details from: {Endpoint}", endpoint);
                throw;
            }
        }

        public async Task AddSoftwareInstanceAsync(SoftwareInstanceRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(BasePath, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding software instance: {Name}", request.Name);
                throw;
            }
        }

        public async Task ModifySoftwareInstanceAsync(string uuid, SoftwareInstanceRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/{Uri.EscapeDataString(uuid)}";
            try
            {
                using var response = await _httpClient.PutAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error modifying software instance: {Uuid}", uuid);
                throw;
            }
        }

        public async Task DeleteSoftwareInstanceAsync(string systemNickname, string swiName, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/{Uri.EscapeDataString(systemNickname)}/{Uri.EscapeDataString(swiName)}";
            try
            {
                using var response = await _httpClient.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting software instance: {Name} on {System}", swiName, systemNickname);
                throw;
            }
        }

        public async Task<string> StartListDataSetsAsync(string uuid, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/{Uri.EscapeDataString(uuid)}/datasets";
            try
            {
                using var response = await _httpClient.PostAsync(endpoint, null, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var status = await response.Content.ReadFromJsonAsync<AsyncStatusResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
                return status?.StatusUrl ?? throw new InvalidOperationException("Status URL not received.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting data set list for instance: {Uuid}", uuid);
                throw;
            }
        }

        public async Task<DatasetListStatusResponse> GetListDataSetsStatusAsync(string statusUrl, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<DatasetListStatusResponse>(statusUrl, cancellationToken).ConfigureAwait(false);
                return response ?? throw new InvalidOperationException("Empty response received from data set list status endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking data set list status at: {Url}", statusUrl);
                throw;
            }
        }

        public async Task<string> StartExportAsync(string uuid, ExportRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/{Uri.EscapeDataString(uuid)}/export";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var status = await response.Content.ReadFromJsonAsync<AsyncStatusResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
                return status?.StatusUrl ?? throw new InvalidOperationException("Status URL not received.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting export for instance: {Uuid}", uuid);
                throw;
            }
        }

        public async Task<ExportStatusResponse> GetExportStatusAsync(string statusUrl, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ExportStatusResponse>(statusUrl, cancellationToken).ConfigureAwait(false);
                return response ?? throw new InvalidOperationException("Empty response received from export status endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking export status at: {Url}", statusUrl);
                throw;
            }
        }
    }
}
