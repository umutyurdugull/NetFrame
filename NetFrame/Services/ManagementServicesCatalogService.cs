using Microsoft.Extensions.Logging;
using NetFrame.Models.ManagementServicesCatalog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class ManagementServicesCatalogService : IManagementServicesCatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ManagementServicesCatalogService> _logger;
        private const string BasePath = "/zosmf/mgmt-services/rest";

        public ManagementServicesCatalogService(HttpClient httpClient, ILogger<ManagementServicesCatalogService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<CatalogServiceSummary>> ListCatalogServicesAsync(string? serviceName = null, string? categoryName = null, string? state = null, bool summary = true, CancellationToken cancellationToken = default)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(serviceName)) queryParams.Add($"serviceName={Uri.EscapeDataString(serviceName)}");
            if (!string.IsNullOrWhiteSpace(categoryName)) queryParams.Add($"categoryName={Uri.EscapeDataString(categoryName)}");
            if (!string.IsNullOrWhiteSpace(state)) queryParams.Add($"state={Uri.EscapeDataString(state)}");
            queryParams.Add($"summary={summary.ToString().ToLower()}");

            var endpoint = $"{BasePath}/catalog-services?" + string.Join("&", queryParams);

            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return new List<CatalogServiceSummary>();

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<CatalogServiceSummary>>(cancellationToken: cancellationToken) ?? new List<CatalogServiceSummary>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing catalog services");
                throw;
            }
        }

        public async Task<string> GetCatalogServiceDetailsAsync(string objectId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(objectId)) throw new ArgumentException("Object ID is required.", nameof(objectId));

            var endpoint = $"{BasePath}/catalog-services/{Uri.EscapeDataString(objectId)}";
            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting catalog service details for {ObjectId}", objectId);
                throw;
            }
        }

        public async Task<List<Category>> ListCategoriesAsync(CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/categories";
            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return new List<Category>();

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<Category>>(cancellationToken: cancellationToken) ?? new List<Category>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing categories");
                throw;
            }
        }

        public async Task<Category> GetCategoryDetailsAsync(string objectId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(objectId)) throw new ArgumentException("Object ID is required.", nameof(objectId));

            var endpoint = $"{BasePath}/categories/{Uri.EscapeDataString(objectId)}";
            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Category>(cancellationToken: cancellationToken) ?? new Category();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category details for {ObjectId}", objectId);
                throw;
            }
        }

        public async Task<List<ServiceSubmissionSummary>> ListServiceSubmissionsAsync(string? serviceName = null, string? status = null, string? submitter = null, string? targetSystem = null, string? label = null, bool summary = true, CancellationToken cancellationToken = default)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(serviceName)) queryParams.Add($"serviceName={Uri.EscapeDataString(serviceName)}");
            if (!string.IsNullOrWhiteSpace(status)) queryParams.Add($"status={Uri.EscapeDataString(status)}");
            if (!string.IsNullOrWhiteSpace(submitter)) queryParams.Add($"submitter={Uri.EscapeDataString(submitter)}");
            if (!string.IsNullOrWhiteSpace(targetSystem)) queryParams.Add($"targetSystem={Uri.EscapeDataString(targetSystem)}");
            if (!string.IsNullOrWhiteSpace(label)) queryParams.Add($"label={Uri.EscapeDataString(label)}");
            queryParams.Add($"summary={summary.ToString().ToLower()}");

            var endpoint = $"{BasePath}/service-instances?" + string.Join("&", queryParams);

            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return new List<ServiceSubmissionSummary>();

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<ServiceSubmissionSummary>>(cancellationToken: cancellationToken) ?? new List<ServiceSubmissionSummary>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing service submissions");
                throw;
            }
        }

        public async Task<string> GetServiceSubmissionDetailsAsync(string objectId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(objectId)) throw new ArgumentException("Object ID is required.", nameof(objectId));

            var endpoint = $"{BasePath}/service-instances/{Uri.EscapeDataString(objectId)}";
            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting service submission details for {ObjectId}", objectId);
                throw;
            }
        }

        public async Task<string> CreateServiceSubmissionAsync(CreateServiceSubmissionRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var endpoint = $"{BasePath}/service-instances";
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating service submission");
                throw;
            }
        }

        public async Task DeleteServiceSubmissionAsync(string objectId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(objectId)) throw new ArgumentException("Object ID is required.", nameof(objectId));

            var endpoint = $"{BasePath}/service-instances/{Uri.EscapeDataString(objectId)}";
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting service submission {ObjectId}", objectId);
                throw;
            }
        }

        public async Task ModifyServiceSubmissionAsync(string objectId, ModifyServiceSubmissionRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(objectId)) throw new ArgumentException("Object ID is required.", nameof(objectId));
            if (request == null) throw new ArgumentNullException(nameof(request));

            var endpoint = $"{BasePath}/service-instances/{Uri.EscapeDataString(objectId)}";
            try
            {
                var reqMsg = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint)
                {
                    Content = JsonContent.Create(request)
                };

                var response = await _httpClient.SendAsync(reqMsg, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error modifying service submission {ObjectId}", objectId);
                throw;
            }
        }

        public async Task PerformActionOnServiceSubmissionAsync(string objectId, string action, ServiceActionRequest? request = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(objectId)) throw new ArgumentException("Object ID is required.", nameof(objectId));
            if (string.IsNullOrWhiteSpace(action)) throw new ArgumentException("Action is required.", nameof(action));

            var endpoint = $"{BasePath}/service-instances/{Uri.EscapeDataString(objectId)}/actions/{Uri.EscapeDataString(action)}";
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, request ?? new ServiceActionRequest(), cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing action {Action} on service submission {ObjectId}", action, objectId);
                throw;
            }
        }

        public async Task<JobStatementResponse> GetJobStatementsAsync(CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/settings/job-statements";
            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<JobStatementResponse>(cancellationToken: cancellationToken) ?? new JobStatementResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting job statements");
                throw;
            }
        }

        public async Task<TargetSystemResponse> GetTargetSystemsAsync(CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/settings/target-systems";
            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TargetSystemResponse>(cancellationToken: cancellationToken) ?? new TargetSystemResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting target systems");
                throw;
            }
        }
    }
}
