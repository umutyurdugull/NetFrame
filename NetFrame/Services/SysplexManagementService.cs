using Microsoft.Extensions.Logging;
using NetFrame.Models.SysplexManagement;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class SysplexManagementService : ISysplexManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SysplexManagementService> _logger;
        private const string Version = "v1";
        private const string BasePath = "/zosmf/sysplex/rest/" + Version;

        public SysplexManagementService(HttpClient httpClient, ILogger<SysplexManagementService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CfrmPolicyListResponse> ListCfrmPoliciesAsync(CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BasePath}/policies/cfrm";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<CfrmPolicyListResponse>(endpoint, cancellationToken);
                return response ?? new CfrmPolicyListResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing CFRM policies.");
                throw;
            }
        }

        public async Task<CfrmPolicyDetailResponse> GetCfrmPolicyAsync(string policyName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(policyName)) throw new ArgumentException("Policy name cannot be empty.", nameof(policyName));

            var endpoint = $"{BasePath}/policies/cfrm/{Uri.EscapeDataString(policyName)}";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<CfrmPolicyDetailResponse>(endpoint, cancellationToken);
                return response ?? throw new InvalidOperationException($"Empty response received for CFRM policy: {policyName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving CFRM policy details: {PolicyName}", policyName);
                throw;
            }
        }
    }
}
