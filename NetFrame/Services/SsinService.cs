using Microsoft.Extensions.Logging;
using NetFrame.Models.ResourceManagement;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class SsinService : ISsinService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SsinService> _logger;
        private const string Version = "1.0";
        private const string BasePath = "/zosmf/resource-mgmt/rest/" + Version;

        public SsinService(HttpClient httpClient, ILogger<SsinService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SsinListResponse> CreateSsinAsync(CreateSsinRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/ssin";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<SsinListResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new SsinListResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating SSIN for registry: {RegistryId}", request.RegistryId);
                throw;
            }
        }

        public async Task<SsinListResponse> ListSsinAsync(string? name = null, string? registryId = null, CancellationToken cancellationToken = default)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(name)) queryParams.Add($"name={Uri.EscapeDataString(name)}");
            if (!string.IsNullOrEmpty(registryId)) queryParams.Add($"registry-id={Uri.EscapeDataString(registryId)}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var endpoint = $"{BasePath}/ssin{queryString}";

            try
            {
                var response = await _httpClient.GetFromJsonAsync<SsinListResponse>(endpoint, cancellationToken).ConfigureAwait(false);
                return response ?? new SsinListResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing SSINs.");
                throw;
            }
        }

        public async Task<VariableNameResponse> CreateVariableNameAsync(CreateVariableNameRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/ssin/variable-name";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<VariableNameResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new VariableNameResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating variable name for registry: {RegistryId}", request.RegistryId);
                throw;
            }
        }
    }
}
