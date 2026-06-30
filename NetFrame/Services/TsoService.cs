using Microsoft.Extensions.Logging;
using NetFrame.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class TsoService : ITsoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TsoService> _logger;

        public TsoService(HttpClient httpClient, ILogger<TsoService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TsoCommandResponse> ExecuteTsoCommandAsync(string command, string? system = null, int? maxWaitTime = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentException("Command cannot be null or empty.", nameof(command));
            }

            var requestBody = new TsoCommandRequest
            {
                TsoCmd = command,
                CmdState = "stateless",
                System = system,
                MaxWaitTime = maxWaitTime
            };

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Put, "/zosmf/tsoApp/v1/tso");
                request.Headers.Add("X-CSRF-ZOSMF-HEADER", "zosmf");
                request.Content = JsonContent.Create(requestBody);

                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                var resContent = await response.Content.ReadFromJsonAsync<TsoCommandResponse>(cancellationToken: cancellationToken);
                return resContent ?? throw new InvalidOperationException("Empty response received from z/OSMF TSO stateless endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing TSO command: {Command}", command);
                throw;
            }
        }
    }
}
