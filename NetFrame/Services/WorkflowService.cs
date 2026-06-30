using Microsoft.Extensions.Logging;
using NetFrame.Models.Workflow;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class WorkflowService : IWorkflowService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WorkflowService> _logger;
        private const string BasePath = "/zosmf/workflow/rest/1.0";

        public WorkflowService(HttpClient httpClient, ILogger<WorkflowService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CreateWorkflowResponse> CreateWorkflowAsync(CreateWorkflowRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var endpoint = $"{BasePath}/workflows";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CreateWorkflowResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new CreateWorkflowResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating workflow.");
                throw;
            }
        }

        public async Task<WorkflowProperties> GetWorkflowPropertiesAsync(string workflowKey, string? returnData = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(workflowKey)) throw new ArgumentException("Workflow key cannot be empty.", nameof(workflowKey));

            var endpoint = $"{BasePath}/workflows/{Uri.EscapeDataString(workflowKey)}";
            if (!string.IsNullOrWhiteSpace(returnData))
            {
                endpoint += $"?returnData={Uri.EscapeDataString(returnData)}";
            }

            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<WorkflowProperties>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new WorkflowProperties();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting properties for workflow {WorkflowKey}", workflowKey);
                throw;
            }
        }

        public async Task<ListWorkflowsResponse> ListWorkflowsAsync(string? workflowName = null, string? system = null, string? owner = null, CancellationToken cancellationToken = default)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(workflowName)) queryParams.Add($"workflowName={Uri.EscapeDataString(workflowName)}");
            if (!string.IsNullOrWhiteSpace(system)) queryParams.Add($"system={Uri.EscapeDataString(system)}");
            if (!string.IsNullOrWhiteSpace(owner)) queryParams.Add($"owner={Uri.EscapeDataString(owner)}");

            var endpoint = $"{BasePath}/workflows";
            if (queryParams.Count > 0)
            {
                endpoint += "?" + string.Join("&", queryParams);
            }

            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ListWorkflowsResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new ListWorkflowsResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing workflows");
                throw;
            }
        }

        public async Task StartWorkflowAsync(string workflowKey, StartWorkflowRequest? request = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(workflowKey)) throw new ArgumentException("Workflow key cannot be empty.", nameof(workflowKey));

            var endpoint = $"{BasePath}/workflows/{Uri.EscapeDataString(workflowKey)}/operations/start";
            request ??= new StartWorkflowRequest();

            try
            {
                using var response = await _httpClient.PutAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting workflow {WorkflowKey}", workflowKey);
                throw;
            }
        }

        public async Task<CancelWorkflowResponse> CancelWorkflowAsync(string workflowKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(workflowKey)) throw new ArgumentException("Workflow key cannot be empty.", nameof(workflowKey));

            var endpoint = $"{BasePath}/workflows/{Uri.EscapeDataString(workflowKey)}/operations/cancel";
            
            try
            {
                using var response = await _httpClient.PutAsJsonAsync(endpoint, new { }, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CancelWorkflowResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new CancelWorkflowResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling workflow {WorkflowKey}", workflowKey);
                throw;
            }
        }

        public async Task DeleteWorkflowAsync(string workflowKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(workflowKey)) throw new ArgumentException("Workflow key cannot be empty.", nameof(workflowKey));

            var endpoint = $"{BasePath}/workflows/{Uri.EscapeDataString(workflowKey)}";
            
            try
            {
                using var response = await _httpClient.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting workflow {WorkflowKey}", workflowKey);
                throw;
            }
        }

        public async Task<string> GetWorkflowDefinitionAsync(string definitionFilePath, string? system = null, string? returnData = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(definitionFilePath)) throw new ArgumentException("Definition file path cannot be empty.", nameof(definitionFilePath));

            var queryParams = new List<string> { $"definitionFilePath={Uri.EscapeDataString(definitionFilePath)}" };
            if (!string.IsNullOrWhiteSpace(system)) queryParams.Add($"workflowDefinitionFileSystem={Uri.EscapeDataString(system)}");
            if (!string.IsNullOrWhiteSpace(returnData)) queryParams.Add($"returnData={Uri.EscapeDataString(returnData)}");

            var endpoint = $"{BasePath}/workflowDefinition?" + string.Join("&", queryParams);

            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow definition from {Path}", definitionFilePath);
                throw;
            }
        }

        public async Task<ArchiveWorkflowResponse> ArchiveWorkflowAsync(string workflowKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(workflowKey)) throw new ArgumentException("Workflow key cannot be empty.", nameof(workflowKey));

            var endpoint = $"{BasePath}/workflows/{Uri.EscapeDataString(workflowKey)}/operations/archive";
            
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, new { }, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ArchiveWorkflowResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new ArchiveWorkflowResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error archiving workflow {WorkflowKey}", workflowKey);
                throw;
            }
        }

        public async Task<ListArchivedWorkflowsResponse> ListArchivedWorkflowsAsync(string? orderBy = null, string? view = null, CancellationToken cancellationToken = default)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(orderBy)) queryParams.Add($"orderBy={Uri.EscapeDataString(orderBy)}");
            if (!string.IsNullOrWhiteSpace(view)) queryParams.Add($"view={Uri.EscapeDataString(view)}");

            var endpoint = $"{BasePath}/archivedworkflows";
            if (queryParams.Count > 0)
            {
                endpoint += "?" + string.Join("&", queryParams);
            }

            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ListArchivedWorkflowsResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new ListArchivedWorkflowsResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing archived workflows");
                throw;
            }
        }

        public async Task<WorkflowProperties> GetArchivedWorkflowPropertiesAsync(string workflowKey, string? returnData = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(workflowKey)) throw new ArgumentException("Workflow key cannot be empty.", nameof(workflowKey));

            var endpoint = $"{BasePath}/archivedworkflows/{Uri.EscapeDataString(workflowKey)}";
            if (!string.IsNullOrWhiteSpace(returnData))
            {
                endpoint += $"?returnData={Uri.EscapeDataString(returnData)}";
            }

            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<WorkflowProperties>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new WorkflowProperties();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting properties for archived workflow {WorkflowKey}", workflowKey);
                throw;
            }
        }

        public async Task DeleteArchivedWorkflowAsync(string workflowKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(workflowKey)) throw new ArgumentException("Workflow key cannot be empty.", nameof(workflowKey));

            var endpoint = $"{BasePath}/archivedworkflows/{Uri.EscapeDataString(workflowKey)}";
            
            try
            {
                using var response = await _httpClient.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting archived workflow {WorkflowKey}", workflowKey);
                throw;
            }
        }
    }
}
