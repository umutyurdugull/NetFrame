using Microsoft.Extensions.Logging;
using NetFrame.Models.Provisioning;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class SoftwareTemplateService : ISoftwareTemplateService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SoftwareTemplateService> _logger;
        private const string Version = "1.0";
        private const string SccPath = "/zosmf/provisioning/rest/" + Version + "/scc";
        private const string PscPath = "/zosmf/provisioning/rest/" + Version + "/psc";

        public SoftwareTemplateService(HttpClient httpClient, ILogger<SoftwareTemplateService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // --- Private Catalog (scc) Operations ---

        public async Task<SoftwareTemplateDetail> CreateTemplateAsync(SoftwareTemplateRequest requestBody, CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(SccPath, requestBody, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<SoftwareTemplateDetail>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new SoftwareTemplateDetail();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating software template: {Name}", requestBody.Name);
                throw;
            }
        }

        public async Task<SoftwareTemplateDetail> GetTemplateAsync(string objectId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{SccPath}/{Uri.EscapeDataString(objectId)}";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<SoftwareTemplateDetail>(endpoint, cancellationToken).ConfigureAwait(false);
                return response ?? throw new InvalidOperationException("Empty response received from get template endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving template: {ObjectId}", objectId);
                throw;
            }
        }

        public async Task<TemplateListResponse> ListTemplatesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<TemplateListResponse>(SccPath, cancellationToken).ConfigureAwait(false);
                return response ?? new TemplateListResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing templates.");
                throw;
            }
        }

        public async Task DeleteTemplateAsync(string objectId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{SccPath}/{Uri.EscapeDataString(objectId)}";
            try
            {
                using var response = await _httpClient.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting template: {ObjectId}", objectId);
                throw;
            }
        }

        public async Task PublishTemplateAsync(string objectId, bool archiveExisting = false, bool ignoreTest = false, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{SccPath}/{Uri.EscapeDataString(objectId)}/actions/publish";
            var body = new { archiveExisting, ignoreTest };
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, body, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing template: {ObjectId}", objectId);
                throw;
            }
        }

        public async Task<RunTemplateResponse> TestTemplateAsync(string objectId, RunTemplateRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{SccPath}/{Uri.EscapeDataString(objectId)}/actions/test";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<RunTemplateResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new RunTemplateResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing template: {ObjectId}", objectId);
                throw;
            }
        }

        // --- Published Catalog (psc) Operations ---

        public async Task<SoftwareTemplateDetail> GetPublishedTemplateAsync(string name, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{PscPath}/{Uri.EscapeDataString(name)}";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<SoftwareTemplateDetail>(endpoint, cancellationToken).ConfigureAwait(false);
                return response ?? throw new InvalidOperationException("Empty response received from get published template endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving published template: {Name}", name);
                throw;
            }
        }

        public async Task<TemplateListResponse> ListPublishedTemplatesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<TemplateListResponse>(PscPath, cancellationToken).ConfigureAwait(false);
                return response ?? new TemplateListResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing published templates.");
                throw;
            }
        }

        public async Task<RunTemplateResponse> RunTemplateAsync(string name, RunTemplateRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{PscPath}/{Uri.EscapeDataString(name)}/actions/run";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<RunTemplateResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new RunTemplateResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running published template: {Name}", name);
                throw;
            }
        }

        // --- Utilities ---

        public async Task<List<PromptVariable>> GetPromptVariablesAsync(string objectId, bool isPublished = false, CancellationToken cancellationToken = default)
        {
            var basePath = isPublished ? PscPath : SccPath;
            var endpoint = $"{basePath}/{Uri.EscapeDataString(objectId)}/prompt-variables";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<Dictionary<string, List<PromptVariable>>>(endpoint, cancellationToken).ConfigureAwait(false);
                if (response != null && response.TryGetValue("prompt-variables", out var variables))
                {
                    return variables;
                }
                return new List<PromptVariable>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prompt variables for template: {ObjectId}", objectId);
                throw;
            }
        }

        public async Task<TemplateHistoryResponse> GetTemplateHistoryAsync(string objectId, bool isPublished = false, CancellationToken cancellationToken = default)
        {
            var basePath = isPublished ? PscPath : SccPath;
            var endpoint = $"{basePath}/{Uri.EscapeDataString(objectId)}/history";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<TemplateHistoryResponse>(endpoint, cancellationToken).ConfigureAwait(false);
                return response ?? new TemplateHistoryResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving history for template: {ObjectId}", objectId);
                throw;
            }
        }
    }
}
