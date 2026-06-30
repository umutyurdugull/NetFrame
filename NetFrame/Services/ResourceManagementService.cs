using Microsoft.Extensions.Logging;
using NetFrame.Models.ResourceManagement;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class ResourceManagementService : IResourceManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ResourceManagementService> _logger;
        private const string Version = "1.0";
        private const string BasePath = "/zosmf/resource-mgmt/rest/" + Version;

        public ResourceManagementService(HttpClient httpClient, ILogger<ResourceManagementService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<DomainDetail> GetDomainAsync(string objectId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(objectId)) throw new ArgumentException("Object ID cannot be empty.", nameof(objectId));

            var endpoint = $"{BasePath}/domains/{Uri.EscapeDataString(objectId)}";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<DomainDetail>(endpoint, cancellationToken).ConfigureAwait(false);
                return response ?? throw new InvalidOperationException("Empty response received from get domain endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving domain: {ObjectId}", objectId);
                throw;
            }
        }

        public async Task<DomainHistoryResponse> GetDomainHistoryAsync(string domainId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(domainId)) throw new ArgumentException("Domain ID cannot be empty.", nameof(domainId));

            var endpoint = $"{BasePath}/domains/{Uri.EscapeDataString(domainId)}/history";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<DomainHistoryResponse>(endpoint, cancellationToken).ConfigureAwait(false);
                return response ?? throw new InvalidOperationException("Empty response received from get domain history endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving history for domain: {DomainId}", domainId);
                throw;
            }
        }

        public async Task<DomainListResponse> ListDomainsAsync(CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/domains/";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<DomainListResponse>(endpoint, cancellationToken).ConfigureAwait(false);
                return response ?? throw new InvalidOperationException("Empty response received from list domains endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing domains.");
                throw;
            }
        }

        public async Task<CreateTenantResponse> CreateTenantAsync(string domainId, CreateTenantRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(domainId)) throw new ArgumentException("Domain ID cannot be empty.", nameof(domainId));
            if (request == null) throw new ArgumentNullException(nameof(request));

            var endpoint = $"{BasePath}/domains/{Uri.EscapeDataString(domainId)}/tenants";
            try
            {
                using var response = await _httpClient.PutAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CreateTenantResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new CreateTenantResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tenant: {TenantName} in domain: {DomainId}", request.TenantName, domainId);
                throw;
            }
        }

        public async Task<TenantDetail> GetTenantAsync(string objectId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(objectId)) throw new ArgumentException("Object ID cannot be empty.", nameof(objectId));

            var endpoint = $"{BasePath}/tenants/{Uri.EscapeDataString(objectId)}";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<TenantDetail>(endpoint, cancellationToken).ConfigureAwait(false);
                return response ?? throw new InvalidOperationException("Empty response received from get tenant endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tenant: {ObjectId}", objectId);
                throw;
            }
        }

        public async Task<DomainHistoryResponse> GetTenantHistoryAsync(string tenantId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentException("Tenant ID cannot be empty.", nameof(tenantId));

            var endpoint = $"{BasePath}/tenants/{Uri.EscapeDataString(tenantId)}/history";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<DomainHistoryResponse>(endpoint, cancellationToken).ConfigureAwait(false);
                return response ?? throw new InvalidOperationException("Empty response received from get tenant history endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving history for tenant: {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<TenantListResponse> ListTenantsAsync(CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/tenants/";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<TenantListResponse>(endpoint, cancellationToken).ConfigureAwait(false);
                return response ?? throw new InvalidOperationException("Empty response received from list tenants endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing tenants.");
                throw;
            }
        }

        public async Task DeleteTenantAsync(string tenantId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentException("Tenant ID cannot be empty.", nameof(tenantId));

            var endpoint = $"{BasePath}/tenants/{Uri.EscapeDataString(tenantId)}";
            try
            {
                using var response = await _httpClient.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tenant: {TenantId}", tenantId);
                throw;
            }
        }

        public async Task AssignCpuCappingPropertiesAsync(string tenantId, AssignCpuCappingRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/tenants/{Uri.EscapeDataString(tenantId)}/actions/assign-cpu-capping-properties";
            await PerformTenantActionAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
        }

        public async Task AssignMemoryCappingPropertiesAsync(string tenantId, AssignMemoryCappingRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/tenants/{Uri.EscapeDataString(tenantId)}/actions/assign-memory-capping-properties";
            await PerformTenantActionAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
        }

        public async Task AssignSolutionIdAsync(string tenantId, AssignSolutionIdRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/tenants/{Uri.EscapeDataString(tenantId)}/actions/assign-solution-id";
            await PerformTenantActionAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
        }

        public async Task DisableCpuCappingAsync(string tenantId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/tenants/{Uri.EscapeDataString(tenantId)}/actions/disable-cpu-capping";
            await PerformTenantActionAsync(endpoint, null, cancellationToken).ConfigureAwait(false);
        }

        public async Task DisableMemoryCappingAsync(string tenantId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/tenants/{Uri.EscapeDataString(tenantId)}/actions/disable-memory-capping";
            await PerformTenantActionAsync(endpoint, null, cancellationToken).ConfigureAwait(false);
        }

        public async Task DisableMeteringAsync(string tenantId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/tenants/{Uri.EscapeDataString(tenantId)}/actions/disable-metering";
            await PerformTenantActionAsync(endpoint, null, cancellationToken).ConfigureAwait(false);
        }

        public async Task EnableCpuCappingAsync(string tenantId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/tenants/{Uri.EscapeDataString(tenantId)}/actions/enable-cpu-capping";
            await PerformTenantActionAsync(endpoint, null, cancellationToken).ConfigureAwait(false);
        }

        public async Task EnableMemoryCappingAsync(string tenantId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/tenants/{Uri.EscapeDataString(tenantId)}/actions/enable-memory-capping";
            await PerformTenantActionAsync(endpoint, null, cancellationToken).ConfigureAwait(false);
        }

        public async Task EnableMeteringAsync(string tenantId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/tenants/{Uri.EscapeDataString(tenantId)}/actions/enable-metering";
            await PerformTenantActionAsync(endpoint, null, cancellationToken).ConfigureAwait(false);
        }

        public async Task AddTenantConsumersAsync(string tenantId, TenantConsumerActionRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/tenants/{Uri.EscapeDataString(tenantId)}/consumers/actions/add";
            await PerformTenantActionAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveTenantConsumersAsync(string tenantId, TenantConsumerActionRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/tenants/{Uri.EscapeDataString(tenantId)}/consumers/actions/remove";
            await PerformTenantActionAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
        }

        public async Task AddTenantDescriptionAsync(string tenantId, TenantDescriptionActionRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/tenants/{Uri.EscapeDataString(tenantId)}/description/actions/add";
            await PerformTenantActionAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
        }

        public async Task AddTenantGroupsAsync(string tenantId, TenantGroupActionRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/tenants/{Uri.EscapeDataString(tenantId)}/groups/actions/add";
            await PerformTenantActionAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveTenantGroupsAsync(string tenantId, TenantGroupActionRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/tenants/{Uri.EscapeDataString(tenantId)}/groups/actions/remove";
            await PerformTenantActionAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ResourcePoolDetail> GetResourcePoolAsync(string tenantId, string rdpId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/tenants/{Uri.EscapeDataString(tenantId)}/rdp/{Uri.EscapeDataString(rdpId)}";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ResourcePoolDetail>(endpoint, cancellationToken).ConfigureAwait(false);
                return response ?? throw new InvalidOperationException("Empty response received from get resource pool endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving resource pool: {RdpId} for tenant: {TenantId}", rdpId, tenantId);
                throw;
            }
        }

        private async Task PerformTenantActionAsync(string endpoint, object? requestBody, CancellationToken cancellationToken)
        {
            try
            {
                HttpResponseMessage response;
                if (requestBody != null)
                {
                    response = await _httpClient.PostAsJsonAsync(endpoint, requestBody, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    response = await _httpClient.PostAsync(endpoint, null, cancellationToken).ConfigureAwait(false);
                }
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing tenant action at: {Endpoint}", endpoint);
                throw;
            }
        }
    }
}
