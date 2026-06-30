using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class RmfMeteringService : IRmfMeteringService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RmfMeteringService> _logger;

        public RmfMeteringService(HttpClient httpClient, ILogger<RmfMeteringService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetMeterDataAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, "/zosmf/izur/rest/meterdata");
                request.Headers.Add("X-CSRF-ZOSMF-HEADER", "zosmf");

                using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false) ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving RMF metering data.");
                throw;
            }
        }
    }
}
