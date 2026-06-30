using Microsoft.Extensions.Logging;
using NetFrame.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class ExternalGatewayService : IExternalGatewayService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExternalGatewayService> _logger;

        public ExternalGatewayService(HttpClient httpClient, ILogger<ExternalGatewayService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ExternalGatewayResponse> GetDataAsync(ExternalGatewayRequest request, CancellationToken cancellationToken = default)
        {
            return await SendGatewayRequestAsync(HttpMethod.Get, request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ExternalGatewayResponse> PostDataAsync(ExternalGatewayRequest request, CancellationToken cancellationToken = default)
        {
            return await SendGatewayRequestAsync(HttpMethod.Post, request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ExternalGatewayResponse> PutDataAsync(ExternalGatewayRequest request, CancellationToken cancellationToken = default)
        {
            return await SendGatewayRequestAsync(HttpMethod.Put, request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ExternalGatewayResponse> DeleteDataAsync(ExternalGatewayRequest request, CancellationToken cancellationToken = default)
        {
            return await SendGatewayRequestAsync(HttpMethod.Delete, request, cancellationToken).ConfigureAwait(false);
        }

        private async Task<ExternalGatewayResponse> SendGatewayRequestAsync(HttpMethod method, ExternalGatewayRequest request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            string endpoint = "/zosmf/externalgateway/system";
            
            HttpRequestMessage httpRequest;

            if (method == HttpMethod.Get || method == HttpMethod.Delete)
            {
                var contentJson = JsonSerializer.Serialize(request, new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
                var uriBuilder = new UriBuilder(new Uri(_httpClient.BaseAddress!, endpoint));
                uriBuilder.Query = $"content={Uri.EscapeDataString(contentJson)}";
                httpRequest = new HttpRequestMessage(method, uriBuilder.Uri);
            }
            else
            {
                httpRequest = new HttpRequestMessage(method, endpoint);
                httpRequest.Content = JsonContent.Create(request, options: new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
            }

            try
            {
                using var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                // If wrapped is "N", the response might not match ExternalGatewayResponse structure.
                // But for now, we assume wrapped "Y" (default) or handle accordingly.
                if (request.Wrapped == "N")
                {
                    // This is a bit tricky since the response is raw from the target server.
                    // For now, let's just return a minimal response or handle as object.
                    var rawContent = await response.Content.ReadFromJsonAsync<object>(cancellationToken: cancellationToken).ConfigureAwait(false);
                    return new ExternalGatewayResponse 
                    { 
                        SystemsOutput = new SystemsOutput { SystemOutput = rawContent, ReturnCode = "Ok" } 
                    };
                }

                return await response.Content.ReadFromJsonAsync<ExternalGatewayResponse>(cancellationToken: cancellationToken).ConfigureAwait(false) ?? new ExternalGatewayResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error routing {Method} request to target: {Target}", method, request.Target);
                throw;
            }
        }
    }
}
