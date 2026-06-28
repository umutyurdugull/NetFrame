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
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class Db2RestService : IDb2RestService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<Db2RestService> _logger;
        private readonly Db2Config _config;
        private string _token;

        public Db2RestService(HttpClient httpClient, IOptions<Db2Config> config, ILogger<Db2RestService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (string.IsNullOrEmpty(_config.DatabaseName))
            {
                var authInfo = $"{_config.Username}:{_config.Password}";
                var byteArray = Encoding.ASCII.GetBytes(authInfo);
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
        }

        private async Task<HttpResponseMessage> PostJsonAsync(string requestUri, object value)
        {
            var json = JsonSerializer.Serialize(value);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(requestUri, content);
        }

        public async Task<string> AuthenticateAsync()
        {
            if (string.IsNullOrEmpty(_config.DatabaseName))
            {
                return null;
            }

            var authPayload = new
            {
                dbConnection = new
                {
                    host = _httpClient.BaseAddress?.Host,
                    port = _httpClient.BaseAddress?.Port,
                    database = _config.DatabaseName,
                    user = _config.Username,
                    password = _config.Password
                }
            };

            try
            {
                var response = await PostJsonAsync("/v1/auth", authPayload);
                response.EnsureSuccessStatusCode();

                var jsonResult = await response.Content.ReadFromJsonAsync<JsonNode>();
                _token = jsonResult?["token"]?.ToString();

                if (string.IsNullOrEmpty(_token))
                {
                    throw new InvalidOperationException("Token was not found in the response payload.");
                }

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _token);

                return _token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Db2 REST authentication.");
                throw;
            }
        }

        public async Task<string> ExecuteSqlAsync(string sqlStatement)
        {
            await EnsureAuthenticatedAsync();

            if (string.IsNullOrEmpty(_config.DatabaseName))
            {
                string tempServiceName = $"TX_{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}";
                try
                {
                    await CreateServiceAsync(tempServiceName, sqlStatement);
                    return await CallServiceAsync(tempServiceName, null);
                }
                finally
                {
                    try
                    {
                        await DeleteServiceAsync(tempServiceName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to clean up temporary service {ServiceName}", tempServiceName);
                    }
                }
            }
            else
            {
                var endpoint = "/v1/services/execsql";
                var payload = new { sqlStmt = sqlStatement };

                try
                {
                    var response = await PostJsonAsync(endpoint, payload);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing ad-hoc SQL statement.");
                    throw;
                }
            }
        }

        public async Task CreateServiceAsync(string serviceName, string sqlStatement, string collectionId = "default")
        {
            await EnsureAuthenticatedAsync();

            if (string.IsNullOrEmpty(_config.DatabaseName))
            {
                var payload = new
                {
                    requestType = "createService",
                    sqlStmt = sqlStatement,
                    collectionID = collectionId,
                    serviceName = serviceName
                };

                var response = await PostJsonAsync("/services/DB2ServiceManager", payload);
                response.EnsureSuccessStatusCode();
            }
            else
            {
                var payload = new
                {
                    serviceName = serviceName,
                    sqlStatement = sqlStatement,
                    version = "1.0"
                };

                var response = await PostJsonAsync("/v1/services", payload);
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task<string> CallServiceAsync(string serviceName, Dictionary<string, object> parameters, string collectionId = "default")
        {
            await EnsureAuthenticatedAsync();

            string endpoint = string.IsNullOrEmpty(_config.DatabaseName)
                ? $"/services/{collectionId}/{serviceName}"
                : $"/v1/services/{serviceName}";

            try
            {
                var response = await PostJsonAsync(endpoint, parameters ?? new Dictionary<string, object>());
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Db2 service {ServiceName}.", serviceName);
                throw;
            }
        }

        public async Task<string> ListServicesAsync()
        {
            await EnsureAuthenticatedAsync();

            if (string.IsNullOrEmpty(_config.DatabaseName))
            {
                var response = await _httpClient.PostAsync("/services/DB2ServiceDiscover", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                var response = await _httpClient.GetAsync("/v1/services");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task DeleteServiceAsync(string serviceName, string collectionId = "default")
        {
            await EnsureAuthenticatedAsync();

            if (string.IsNullOrEmpty(_config.DatabaseName))
            {
                var payload = new
                {
                    requestType = "dropService",
                    collectionID = collectionId,
                    serviceName = serviceName
                };

                var response = await PostJsonAsync("/services/DB2ServiceManager", payload);
                response.EnsureSuccessStatusCode();
            }
            else
            {
                var response = await _httpClient.DeleteAsync($"/v1/services/{serviceName}");
                response.EnsureSuccessStatusCode();
            }
        }

        private async Task EnsureAuthenticatedAsync()
        {
            if (!string.IsNullOrEmpty(_config.DatabaseName) && string.IsNullOrEmpty(_token))
            {
                await AuthenticateAsync();
            }
        }
    }
}