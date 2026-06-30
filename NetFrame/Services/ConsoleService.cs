using Microsoft.Extensions.Logging;
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

        public ConsoleService(HttpClient httpClient, ILogger<ConsoleService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Put, "/zosmf/restconsoles/consoles/defcn");
                request.Headers.Add("X-CSRF-ZOSMF-HEADER", "zosmf");
                request.Content = JsonContent.Create(requestBody);

                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                var cmdResponse = await response.Content.ReadFromJsonAsync<ConsoleCommandResponse>(cancellationToken: cancellationToken);
                if (cmdResponse == null || string.IsNullOrEmpty(cmdResponse.CmdResponseUrl))
                {
                    throw new InvalidOperationException("Failed to retrieve command response URL from z/OSMF console service.");
                }

                string responseUrl = cmdResponse.CmdResponseUrl;
                string resultText = string.Empty;

                for (int i = 0; i < 15; i++)
                {
                    await Task.Delay(1000, cancellationToken);

                    using var getRequest = new HttpRequestMessage(HttpMethod.Get, responseUrl);
                    getRequest.Headers.Add("X-CSRF-ZOSMF-HEADER", "zosmf");

                    var getResponse = await _httpClient.SendAsync(getRequest, cancellationToken);
                    getResponse.EnsureSuccessStatusCode();

                    var msgs = await getResponse.Content.ReadFromJsonAsync<ConsoleResponseMessages>(cancellationToken: cancellationToken);
                    if (msgs != null && !string.IsNullOrEmpty(msgs.CmdResponse))
                    {
                        resultText = msgs.CmdResponse;
                        break;
                    }
                }

                return resultText;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error issuing console command: {Command}", command);
                throw;
            }
        }
    }
}
