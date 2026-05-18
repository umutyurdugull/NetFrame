using Microsoft.Extensions.Logging;
using NetFrame.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class SystemService : ISystemService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SystemService> _logger;

        public SystemService(HttpClient httpClient, ILogger<SystemService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ZosmfInfoResponse> GetInfoAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ZosmfInfoResponse>("/zosmf/info", cancellationToken);
                return response ?? throw new InvalidOperationException("Empty response received from z/OSMF info endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving z/OSMF system info.");
                throw;
            }
        }
    }
}
