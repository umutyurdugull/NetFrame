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

        public TsoService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<TsoCommandResponse> ExecuteTsoCommandAsync(
            string command, 
            string? system = null, 
            int? maxWaitTime = null, 
            CancellationToken cancellationToken = default)
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

            using var request = new HttpRequestMessage(HttpMethod.Put, "/zosmf/tsoApp/v1/tso");
            request.Content = JsonContent.Create(requestBody);

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var resContent = await response.Content.ReadFromJsonAsync<TsoCommandResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
            return resContent ?? throw new InvalidOperationException("Empty response received from z/OSMF TSO stateless endpoint.");
        }
    }
}
