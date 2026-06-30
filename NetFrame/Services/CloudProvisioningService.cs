using Microsoft.Extensions.Logging;
using NetFrame.Models.CloudProvisioning;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class CloudProvisioningService : ICloudProvisioningService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CloudProvisioningService> _logger;
        private const string Version = "1.0";
        private const string BasePath = "/zosmf/resource-mgmt/rest/" + Version + "/rdp/network";

        public CloudProvisioningService(HttpClient httpClient, ILogger<CloudProvisioningService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ObtainIpResponse> ObtainIpAddressAsync(ResourcePoolRequest<ObtainIpParams> request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/ip/actions/obtain";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ObtainIpResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new ObtainIpResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obtaining IP address for template: {TemplateName}", request.TemplateName);
                throw;
            }
        }

        public async Task ReleaseIpAddressAsync(ResourcePoolRequest<ReleaseIpParams> request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/ip/actions/release";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing IP address ID: {IpId}", request.NetworkParams.IpId);
                throw;
            }
        }

        public async Task<ObtainPortResponse> ObtainPortAsync(ResourcePoolRequest<ObtainPortParams> request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/port/actions/obtain";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ObtainPortResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new ObtainPortResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obtaining port for template: {TemplateName}", request.TemplateName);
                throw;
            }
        }

        public async Task ReleasePortAsync(ResourcePoolRequest<ReleasePortParams> request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/port/actions/release";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing port ID: {PortId}", request.NetworkParams.PortId);
                throw;
            }
        }

        public async Task<ObtainSnaResponse> ObtainSnaApplicationNameAsync(ResourcePoolRequest<ObtainSnaParams> request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/snaapplname/actions/obtain";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ObtainSnaResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new ObtainSnaResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obtaining SNA application name for template: {TemplateName}", request.TemplateName);
                throw;
            }
        }

        public async Task ReleaseSnaApplicationNameAsync(ResourcePoolRequest<ReleaseSnaParams> request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/snaapplname/actions/release";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing SNA application name ID: {SnaId}", request.NetworkParams.ApplNameId);
                throw;
            }
        }

        public async Task<AddClassificationRuleResponse> AddWlmClassificationRuleAsync(WlmClassificationRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = "/zosmf/resource-mgmt/rest/" + Version + "/rdp/wlm/clrule/actions/add";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<AddClassificationRuleResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new AddClassificationRuleResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding WLM classification rule for template: {TemplateName}", request.TemplateName);
                throw;
            }
        }

        public async Task RemoveWlmClassificationRuleAsync(WlmClassificationRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = "/zosmf/resource-mgmt/rest/" + Version + "/rdp/wlm/clrule/actions/remove";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing WLM classification rule ID: {ClRuleId}", request.WlmParams.ClRuleId);
                throw;
            }
        }

        public async Task<DatasetAttributesResponse> GetDatasetAttributesAsync(string tenantId, string templateName, string registryUuid, string? dsnType = null, string? size = null, CancellationToken cancellationToken = default)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(dsnType)) queryParams.Add($"dsn-type={Uri.EscapeDataString(dsnType)}");
            if (!string.IsNullOrEmpty(size)) queryParams.Add($"size={Uri.EscapeDataString(size)}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var endpoint = $"/zosmf/resource-mgmt/rest/{Version}/rdp/storage/dataset-attr/{Uri.EscapeDataString(tenantId)}/{Uri.EscapeDataString(templateName)}/{Uri.EscapeDataString(registryUuid)}{queryString}";

            try
            {
                var response = await _httpClient.GetFromJsonAsync<DatasetAttributesResponse>(endpoint, cancellationToken).ConfigureAwait(false);
                return response ?? throw new InvalidOperationException("Empty response received from get dataset attributes endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dataset attributes for tenant: {TenantId}, template: {TemplateName}", tenantId, templateName);
                throw;
            }
        }

        public async Task<CreateLparEntryResponse> CreateLparResourcePoolEntryAsync(string rdpId, LparEntryRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"/zosmf/resource-mgmt/rest/{Version}/rdp/{Uri.EscapeDataString(rdpId)}/lpar/";
            try
            {
                using var response = await _httpClient.PutAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CreateLparEntryResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new CreateLparEntryResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating LPAR entry in resource pool: {RdpId}", rdpId);
                throw;
            }
        }

        public async Task ModifyLparResourcePoolEntryAsync(string rdpId, string lparPoolId, LparEntryRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"/zosmf/resource-mgmt/rest/{Version}/rdp/{Uri.EscapeDataString(rdpId)}/lpar/{Uri.EscapeDataString(lparPoolId)}";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error modifying LPAR entry: {LparPoolId} in resource pool: {RdpId}", lparPoolId, rdpId);
                throw;
            }
        }

        public async Task DeleteLparResourcePoolEntryAsync(string rdpId, string lparPoolId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"/zosmf/resource-mgmt/rest/{Version}/rdp/{Uri.EscapeDataString(rdpId)}/lpar/{Uri.EscapeDataString(lparPoolId)}";
            try
            {
                using var response = await _httpClient.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting LPAR entry: {LparPoolId} from resource pool: {RdpId}", lparPoolId, rdpId);
                throw;
            }
        }

        public async Task<List<LparEntryDetail>> ListLparResourcePoolEntriesAsync(string rdpId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"/zosmf/resource-mgmt/rest/{Version}/rdp/{Uri.EscapeDataString(rdpId)}/lpar/";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<LparEntryDetail>>(endpoint, cancellationToken).ConfigureAwait(false);
                return response ?? new List<LparEntryDetail>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing LPAR entries for resource pool: {RdpId}", rdpId);
                throw;
            }
        }

        public async Task<LparEntryDetail> GetLparResourcePoolEntryAsync(string rdpId, string lparPoolId, CancellationToken cancellationToken = default)
        {
            var endpoint = $"/zosmf/resource-mgmt/rest/{Version}/rdp/{Uri.EscapeDataString(rdpId)}/lpar/{Uri.EscapeDataString(lparPoolId)}";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<LparEntryDetail>(endpoint, cancellationToken).ConfigureAwait(false);
                return response ?? throw new InvalidOperationException("Empty response received from get LPAR entry properties endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving properties for LPAR entry: {LparPoolId}", lparPoolId);
                throw;
            }
        }

        public async Task<LparEntryDetail> ObtainLparResourcePoolEntryAsync(ObtainLparRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"/zosmf/resource-mgmt/rest/{Version}/rdp/lpar/actions/obtain";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<LparEntryDetail>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new LparEntryDetail();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obtaining LPAR entry for template: {TemplateName}", request.TemplateName);
                throw;
            }
        }

        public async Task ReleaseLparResourcePoolEntryAsync(ReleaseLparRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = $"/zosmf/resource-mgmt/rest/{Version}/rdp/lpar/actions/release";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing LPAR entry ID: {LparPoolId}", request.LparParams.LparPoolId);
                throw;
            }
        }
    }
}
