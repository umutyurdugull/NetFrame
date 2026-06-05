using Microsoft.Extensions.Logging;
using NetFrame.Models.StorageManagement;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class StorageManagementService : IStorageManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StorageManagementService> _logger;
        private const string Version = "v1";
        private const string BasePath = "/zosmf/storage/rest/" + Version;

        public StorageManagementService(HttpClient httpClient, ILogger<StorageManagementService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<StorageGroupSummary>> ListStorageGroupsAsync(string? filter = null, string? type = null, bool detailData = false, CancellationToken cancellationToken = default)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(filter)) queryParams.Add($"filter={Uri.EscapeDataString(filter)}");
            if (!string.IsNullOrEmpty(type)) queryParams.Add($"type={Uri.EscapeDataString(type)}");
            if (detailData) queryParams.Add("detail-data=Y");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var endpoint = $"{BasePath}/storagegroups{queryString}";

            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<StorageGroupSummary>>(endpoint, cancellationToken);
                return response ?? new List<StorageGroupSummary>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing storage groups.");
                throw;
            }
        }

        public async Task<StorageGroupDetail> GetStorageGroupDefinitionAsync(string scdsName, string stgName, bool detailData = false, CancellationToken cancellationToken = default)
        {
            var queryParams = new List<string> { $"stg-name={Uri.EscapeDataString(stgName)}" };
            if (detailData) queryParams.Add("detail-data=Y");

            var queryString = "?" + string.Join("&", queryParams);
            var endpoint = $"{BasePath}/cds/{Uri.EscapeDataString(scdsName)}{queryString}";

            try
            {
                // The API returns an array even for a single item according to documentation examples
                var response = await _httpClient.GetFromJsonAsync<List<StorageGroupDetail>>(endpoint, cancellationToken);
                if (response != null && response.Count > 0)
                {
                    return response[0];
                }
                throw new InvalidOperationException($"Storage group {stgName} not found in SCDS {scdsName}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving storage group definition: {StgName}", stgName);
                throw;
            }
        }

        public async Task<List<VolumeSummary>> ListVolumesAsync(string? filter = null, string? stgName = null, int offset = 0, int limit = 0, CancellationToken cancellationToken = default)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(filter)) queryParams.Add($"filter={Uri.EscapeDataString(filter)}");
            if (!string.IsNullOrEmpty(stgName)) queryParams.Add($"stg-name={Uri.EscapeDataString(stgName)}");
            if (offset > 0) queryParams.Add($"offset={offset}");
            if (limit > 0) queryParams.Add($"limit={limit}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var endpoint = $"{BasePath}/volumes{queryString}";

            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<VolumeSummary>>(endpoint, cancellationToken);
                return response ?? new List<VolumeSummary>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing volumes.");
                throw;
            }
        }

        public async Task<VolumeSummary> GetVolumeDefinitionAsync(string volumeSer, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/volumes/{Uri.EscapeDataString(volumeSer)}";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<VolumeSummary>>(endpoint, cancellationToken);
                if (response != null && response.Count > 0)
                {
                    return response[0];
                }
                throw new InvalidOperationException($"Volume {volumeSer} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving volume definition: {VolumeSer}", volumeSer);
                throw;
            }
        }

        public async Task<List<DataClassSummary>> ListDataClassesAsync(string? filter = null, bool detailData = false, int offset = 0, int limit = 0, CancellationToken cancellationToken = default)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(filter)) queryParams.Add($"filter={Uri.EscapeDataString(filter)}");
            if (detailData) queryParams.Add("detail-data=Y");
            if (offset > 0) queryParams.Add($"offset={offset}");
            if (limit > 0) queryParams.Add($"limit={limit}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var endpoint = $"{BasePath}/dataclasses{queryString}";

            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<DataClassSummary>>(endpoint, cancellationToken);
                return response ?? new List<DataClassSummary>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing data classes.");
                throw;
            }
        }
    }
}
