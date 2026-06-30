using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetFrame.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class Db2RestService : IDb2RestService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<Db2RestService> _logger;
        private readonly Db2Config _config;
        private readonly IDb2TokenStore _tokenStore;

        public Db2RestService(
            HttpClient httpClient, 
            IOptions<Db2Config> config, 
            ILogger<Db2RestService> logger,
            IDb2TokenStore tokenStore)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
            _tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string requestUri, object? payload = null)
        {
            var request = new HttpRequestMessage(method, requestUri);
            if (payload != null)
            {
                var json = JsonSerializer.Serialize(payload);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            if (!string.IsNullOrEmpty(_config.DatabaseName))
            {
                var token = await _tokenStore.GetOrAuthenticateAsync(AuthenticateInternalAsync).ConfigureAwait(false);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else if (!string.IsNullOrEmpty(_config.Username))
            {
                var authInfo = $"{_config.Username}:{_config.Password}";
                var byteArray = Encoding.ASCII.GetBytes(authInfo);
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }

            return request;
        }

        private async Task<string> AuthenticateInternalAsync()
        {
            var host = _httpClient.BaseAddress?.Host;
            var port = _httpClient.BaseAddress?.Port ?? -1;
            if (port == -1 && _httpClient.BaseAddress != null)
            {
                port = _httpClient.BaseAddress.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) ? 443 : 80;
            }

            var authPayload = new
            {
                dbConnection = new
                {
                    host = host,
                    port = port,
                    database = _config.DatabaseName,
                    user = _config.Username,
                    password = _config.Password
                }
            };

            using var response = await _httpClient.PostAsJsonAsync("/v1/auth", authPayload).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var jsonResult = await response.Content.ReadFromJsonAsync<JsonNode>().ConfigureAwait(false);
            var token = jsonResult?["token"]?.ToString();

            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("Authentication token was not found in the response payload.");
            }

            return token;
        }

        public async Task<string?> AuthenticateAsync()
        {
            if (string.IsNullOrEmpty(_config.DatabaseName))
            {
                return null;
            }
            return await _tokenStore.GetOrAuthenticateAsync(AuthenticateInternalAsync).ConfigureAwait(false);
        }

        public async Task<string> ExecuteSqlAsync(string sqlStatement)
        {
            if (string.IsNullOrEmpty(_config.DatabaseName))
            {
                string tempServiceName = $"TX_{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}";
                try
                {
                    await CreateServiceAsync(tempServiceName, sqlStatement).ConfigureAwait(false);
                    return await CallServiceAsync(tempServiceName, null).ConfigureAwait(false);
                }
                finally
                {
                    try
                    {
                        await DeleteServiceAsync(tempServiceName).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to clean up temporary service {ServiceName}", tempServiceName);
                    }
                }
            }
            else
            {
                var payload = new { sqlStmt = sqlStatement };
                using var request = await CreateRequestAsync(HttpMethod.Post, "/v1/services/execsql", payload).ConfigureAwait(false);
                using var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _tokenStore.InvalidateToken();
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }

        public async Task CreateServiceAsync(string serviceName, string sqlStatement, string collectionId = "default")
        {
            object payload;
            string endpoint;

            if (string.IsNullOrEmpty(_config.DatabaseName))
            {
                endpoint = "/services/DB2ServiceManager";
                payload = new
                {
                    requestType = "createService",
                    sqlStmt = sqlStatement,
                    collectionID = collectionId,
                    serviceName = serviceName
                };
            }
            else
            {
                endpoint = "/v1/services";
                payload = new
                {
                    serviceName = serviceName,
                    sqlStatement = sqlStatement,
                    version = "1.0"
                };
            }

            using var request = await CreateRequestAsync(HttpMethod.Post, endpoint, payload).ConfigureAwait(false);
            using var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _tokenStore.InvalidateToken();
            }

            response.EnsureSuccessStatusCode();
        }

        public async Task<string> CallServiceAsync(string serviceName, Dictionary<string, object>? parameters, string collectionId = "default")
        {
            string endpoint = string.IsNullOrEmpty(_config.DatabaseName)
                ? $"/services/{collectionId}/{serviceName}"
                : $"/v1/services/{serviceName}";

            using var request = await CreateRequestAsync(HttpMethod.Post, endpoint, parameters ?? new Dictionary<string, object>()).ConfigureAwait(false);
            using var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _tokenStore.InvalidateToken();
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public async Task<string> ListServicesAsync()
        {
            string endpoint;
            HttpMethod method;
            object? payload = null;

            if (string.IsNullOrEmpty(_config.DatabaseName))
            {
                endpoint = "/services/DB2ServiceDiscover";
                method = HttpMethod.Post;
                payload = new { };
            }
            else
            {
                endpoint = "/v1/services";
                method = HttpMethod.Get;
            }

            using var request = await CreateRequestAsync(method, endpoint, payload).ConfigureAwait(false);
            using var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _tokenStore.InvalidateToken();
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public async Task DeleteServiceAsync(string serviceName, string collectionId = "default")
        {
            string endpoint;
            HttpMethod method;
            object? payload = null;

            if (string.IsNullOrEmpty(_config.DatabaseName))
            {
                endpoint = "/services/DB2ServiceManager";
                method = HttpMethod.Post;
                payload = new
                {
                    requestType = "dropService",
                    collectionID = collectionId,
                    serviceName = serviceName
                };
            }
            else
            {
                endpoint = $"/v1/services/{serviceName}";
                method = HttpMethod.Delete;
            }

            using var request = await CreateRequestAsync(method, endpoint, payload).ConfigureAwait(false);
            using var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _tokenStore.InvalidateToken();
            }

            response.EnsureSuccessStatusCode();
        }
    }
}