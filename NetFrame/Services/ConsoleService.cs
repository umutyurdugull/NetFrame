using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetFrame.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class ConsoleService : IConsoleService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ConsoleService> _logger;
        private readonly ZosmfConfig _config;

        public ConsoleService(
            HttpClient httpClient, 
            ILogger<ConsoleService> logger, 
            IOptions<ZosmfConfig> config)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<string> IssueCommandAsync(string command, string? system = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentException("Command cannot be null or empty.", nameof(command));
            }

            var requestBody = new ConsoleCommandRequest
            {
                Cmd = command,
                System = system
            };

            using var request = new HttpRequestMessage(HttpMethod.Put, "/zosmf/restconsoles/consoles/defcn");
            request.Content = JsonContent.Create(requestBody);

            string responseUrl;
            using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                var cmdResponse = await response.Content.ReadFromJsonAsync<ConsoleCommandResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
                if (cmdResponse == null || string.IsNullOrEmpty(cmdResponse.CmdResponseUrl))
                {
                    throw new InvalidOperationException("Failed to retrieve command response URL from z/OSMF console service.");
                }
                responseUrl = cmdResponse.CmdResponseUrl;
            }

            string resultText = string.Empty;
            var random = new Random();

            for (int i = 0; i < _config.MaxPollingAttempts; i++)
            {
                double factor = Math.Pow(_config.PollingBackoffFactor, i);
                double baseDelayMs = _config.PollingIntervalSeconds * 1000.0 * factor;
                int jitterMs = random.Next(0, 500);
                int totalDelayMs = (int)Math.Min(baseDelayMs + jitterMs, 15000);

                _logger.LogInformation("Polling console command response. Attempt {Attempt}/{Max}. Waiting {Delay}ms.", 
                    i + 1, _config.MaxPollingAttempts, totalDelayMs);

                await Task.Delay(totalDelayMs, cancellationToken).ConfigureAwait(false);

                using var getRequest = new HttpRequestMessage(HttpMethod.Get, responseUrl);
                using var getResponse = await _httpClient.SendAsync(getRequest, cancellationToken).ConfigureAwait(false);
                getResponse.EnsureSuccessStatusCode();

                var msgs = await getResponse.Content.ReadFromJsonAsync<ConsoleResponseMessages>(cancellationToken: cancellationToken).ConfigureAwait(false);
                if (msgs != null && !string.IsNullOrEmpty(msgs.CmdResponse))
                {
                    resultText = msgs.CmdResponse;
                    break;
                }
            }

            return resultText;
        }
    }
}
