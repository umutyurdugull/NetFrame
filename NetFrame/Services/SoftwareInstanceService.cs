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
    public class SoftwareInstanceService : ISoftwareInstanceService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SoftwareInstanceService> _logger;
        private const string Version = "1.0";
        private const string BasePath = "/zosmf/provisioning/rest/" + Version + "/scr";

        public SoftwareInstanceService(HttpClient httpClient, ILogger<SoftwareInstanceService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SoftwareInstanceDetail> CreateInstanceAsync(SoftwareInstanceRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BasePath, request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<SoftwareInstanceDetail>(cancellationToken: cancellationToken) ?? new SoftwareInstanceDetail();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating software instance: {Type}", request.Type);
                throw;
            }
        }

        public async Task<SoftwareInstanceDetail> GetInstanceAsync(string objectId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/{Uri.EscapeDataString(objectId)}";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<SoftwareInstanceDetail>(endpoint, cancellationToken);
                return response ?? throw new InvalidOperationException("Empty response received from get instance endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving instance: {ObjectId}", objectId);
                throw;
            }
        }

        public async Task<InstanceListResponse> ListInstancesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<InstanceListResponse>(BasePath, cancellationToken);
                return response ?? new InstanceListResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing instances.");
                throw;
            }
        }

        public async Task UpdateInstanceAsync(string objectId, SoftwareInstanceRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/{Uri.EscapeDataString(objectId)}";
            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, request, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating instance: {ObjectId}", objectId);
                throw;
            }
        }

        public async Task DeleteInstanceAsync(string objectId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/{Uri.EscapeDataString(objectId)}";
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting instance: {ObjectId}", objectId);
                throw;
            }
        }

        public async Task<ActionResult> PerformActionAsync(string objectId, string actionName, PerformActionRequest? request = null, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/{Uri.EscapeDataString(objectId)}/actions/{Uri.EscapeDataString(actionName)}";
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, request ?? new PerformActionRequest(), cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ActionResult>(cancellationToken: cancellationToken) ?? new ActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing action {Action} on instance: {ObjectId}", actionName, objectId);
                throw;
            }
        }

        public async Task<ActionResult> GetActionResponseAsync(string objectId, string actionId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/{Uri.EscapeDataString(objectId)}/actions/{Uri.EscapeDataString(actionId)}";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ActionResult>(endpoint, cancellationToken);
                return response ?? throw new InvalidOperationException("Empty response received from get action response endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving action response for action: {ActionId} on instance: {ObjectId}", actionId, objectId);
                throw;
            }
        }

        public async Task<ActionResponseList> ListActionResponsesAsync(string objectId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/{Uri.EscapeDataString(objectId)}/actions";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ActionResponseList>(endpoint, cancellationToken);
                return response ?? new ActionResponseList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing action responses for instance: {ObjectId}", objectId);
                throw;
            }
        }

        public async Task ResumeProvisioningWorkflowAsync(string objectId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/{Uri.EscapeDataString(objectId)}/resume-workflow";
            try
            {
                var response = await _httpClient.PostAsync(endpoint, null, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resuming provisioning workflow for instance: {ObjectId}", objectId);
                throw;
            }
        }

        public async Task RetryProvisioningWorkflowAsync(string objectId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/{Uri.EscapeDataString(objectId)}/retry-workflow";
            try
            {
                var response = await _httpClient.PostAsync(endpoint, null, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrying provisioning workflow for instance: {ObjectId}", objectId);
                throw;
            }
        }

        public async Task ResumeActionWorkflowAsync(string objectId, string actionId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/{Uri.EscapeDataString(objectId)}/actions/{Uri.EscapeDataString(actionId)}/resume-workflow";
            try
            {
                var response = await _httpClient.PostAsync(endpoint, null, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resuming action workflow: {ActionId} for instance: {ObjectId}", actionId, objectId);
                throw;
            }
        }

        public async Task RetryActionWorkflowAsync(string objectId, string actionId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/{Uri.EscapeDataString(objectId)}/actions/{Uri.EscapeDataString(actionId)}/retry-workflow";
            try
            {
                var response = await _httpClient.PostAsync(endpoint, null, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrying action workflow: {ActionId} for instance: {ObjectId}", actionId, objectId);
                throw;
            }
        }

        public async Task UpdateVariablesAsync(string objectId, List<InstanceVariable> variables, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/{Uri.EscapeDataString(objectId)}/variables";
            var body = new { variables };
            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, body, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating variables for instance: {ObjectId}", objectId);
                throw;
            }
        }
    }
}
