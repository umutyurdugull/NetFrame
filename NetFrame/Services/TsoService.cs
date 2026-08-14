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

        public async Task<TsoCommandResponse> StartInteractiveTsoSessionAsync(string? logonProcedure = null, CancellationToken cancellationToken = default)
        {
            var procName = string.IsNullOrWhiteSpace(logonProcedure) ? "IKJACCNT" : logonProcedure;
            var payload = new
            {
                proc = procName,
                chset = "697",
                cpage = "1047"
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "/zosmf/tso/v1/tso");
            request.Content = JsonContent.Create(payload);

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                using var altReq = new HttpRequestMessage(HttpMethod.Post, "/zosmf/tsoApp/v1/tso");
                altReq.Content = JsonContent.Create(payload);
                using var altRes = await _httpClient.SendAsync(altReq, cancellationToken).ConfigureAwait(false);
                altRes.EnsureSuccessStatusCode();
                var altContent = await altRes.Content.ReadFromJsonAsync<TsoCommandResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
                return altContent ?? throw new InvalidOperationException("Failed to start TSO interactive session.");
            }

            var resContent = await response.Content.ReadFromJsonAsync<TsoCommandResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
            return resContent ?? throw new InvalidOperationException("Failed to start TSO interactive session.");
        }

        public async Task<TsoCommandResponse> SendTsoSessionInputAsync(string servletKey, string commandData, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(servletKey))
            {
                throw new ArgumentException("Servlet key cannot be null or empty.", nameof(servletKey));
            }

            var url = $"/zosmf/tso/v1/tso/{servletKey}";
            var payload = new
            {
                tsoData = commandData ?? string.Empty
            };

            using var request = new HttpRequestMessage(HttpMethod.Put, url);
            request.Content = JsonContent.Create(payload);

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var altUrl = $"/zosmf/tsoApp/v1/tso/{servletKey}";
                using var altReq = new HttpRequestMessage(HttpMethod.Put, altUrl);
                altReq.Content = JsonContent.Create(payload);
                using var altRes = await _httpClient.SendAsync(altReq, cancellationToken).ConfigureAwait(false);
                altRes.EnsureSuccessStatusCode();
                var altContent = await altRes.Content.ReadFromJsonAsync<TsoCommandResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
                return altContent ?? throw new InvalidOperationException("Empty response received from interactive TSO session.");
            }

            var resContent = await response.Content.ReadFromJsonAsync<TsoCommandResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
            return resContent ?? throw new InvalidOperationException("Empty response received from interactive TSO session.");
        }

        public async Task EndTsoSessionAsync(string servletKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(servletKey))
            {
                throw new ArgumentException("Servlet key cannot be null or empty.", nameof(servletKey));
            }

            var url = $"/zosmf/tso/v1/tso/{servletKey}";
            using var request = new HttpRequestMessage(HttpMethod.Delete, url);

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var altUrl = $"/zosmf/tsoApp/v1/tso/{servletKey}";
                using var altReq = new HttpRequestMessage(HttpMethod.Delete, altUrl);
                using var altRes = await _httpClient.SendAsync(altReq, cancellationToken).ConfigureAwait(false);
                altRes.EnsureSuccessStatusCode();
            }
        }
    }
}
