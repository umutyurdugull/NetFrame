using Microsoft.Extensions.Logging;
using NetFrame.Models.WlmResourcePooling;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class WlmResourcePoolingService : IWlmResourcePoolingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WlmResourcePoolingService> _logger;
        private const string BasePath = "/zosmf/zwlm/rest";

        public WlmResourcePoolingService(HttpClient httpClient, ILogger<WlmResourcePoolingService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PrimeWrpResponse> PrimeWlmResourcePoolAsync(PrimeWrpRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var endpoint = $"{BasePath}/wrps";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<PrimeWrpResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new PrimeWrpResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error priming WLM resource pool: {WrpName}", request.WrpData?.WrpName);
                throw;
            }
        }

        public async Task<DeleteWrpResponse> DeleteWlmResourcePoolAsync(string wrpId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(wrpId)) throw new ArgumentException("WRP ID cannot be empty.", nameof(wrpId));

            var endpoint = $"{BasePath}/wrps/{Uri.EscapeDataString(wrpId)}";
            try
            {
                using var response = await _httpClient.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                // The documentation shows it returns a JSON body with status, return-code, message
                return await response.Content.ReadFromJsonAsync<DeleteWrpResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new DeleteWrpResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting WLM resource pool: {WrpId}", wrpId);
                throw;
            }
        }

        public async Task<ConstructPolicyResponse> ConstructWlmServiceDefinitionAsync(ConstructPolicyRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var endpoint = $"{BasePath}/policy/inspolicy";
            try
            {
                using var response = await _httpClient.PutAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ConstructPolicyResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new ConstructPolicyResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error constructing WLM service definition for WRP ID: {WrpId}", request.CloudInfo?.WrpId);
                throw;
            }
        }
    }
}
