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
        private readonly IJobService? _jobService;

        public Db2RestService(
            HttpClient httpClient, 
            IOptions<Db2Config> config, 
            ILogger<Db2RestService> logger,
            IDb2TokenStore tokenStore,
            IJobService? jobService = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
            _tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            _jobService = jobService;

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

        public async Task<string> ExecuteSqlViaJclAsync(string sqlStatement, string? jobCard = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sqlStatement))
            {
                throw new ArgumentException("SQL statement cannot be empty.", nameof(sqlStatement));
            }

            string defaultJobCard = "//DB2SQL   JOB CLASS=A,MSGCLASS=X,NOTIFY=&SYSUID\n";
            string jclHeader = string.IsNullOrWhiteSpace(jobCard) ? defaultJobCard : jobCard;

            string formattedSql = sqlStatement.Trim();
            if (!formattedSql.EndsWith(";"))
                formattedSql += ";";

            var wrappedSql = new StringBuilder();
            var tokens = formattedSql.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var currentLine = new StringBuilder();

            foreach (var token in tokens)
            {
                if (currentLine.Length + token.Length + 1 > 70)
                {
                    wrappedSql.AppendLine(currentLine.ToString());
                    currentLine.Clear();
                }
                if (currentLine.Length > 0)
                    currentLine.Append(' ');
                currentLine.Append(token);
            }
            if (currentLine.Length > 0)
            {
                wrappedSql.AppendLine(currentLine.ToString());
            }

            string jcl = $"{jclHeader}" +
                         "//STEP1    EXEC PGM=IKJEFT01,DYNAMNBR=20\n" +
                         "//SYSTSPRT DD SYSOUT=*\n" +
                         "//SYSTSIN  DD *\n" +
                         " DSN SYSTEM(DB2P)\n" +
                         " RUN PROGRAM(DSNTEP2) PLAN(DSNTEP12)\n" +
                         " END\n" +
                         "/*\n" +
                         "//SYSIN    DD *\n" +
                         $"{wrappedSql}" +
                         "/*\n";

            if (_jobService != null)
            {
                var jobOptions = new JobSubmissionOptions { JclContent = jcl };
                var statusJson = await _jobService.SubmitJobAndWaitAsync(jobOptions, cancellationToken).ConfigureAwait(false);

                try
                {
                    using var doc = JsonDocument.Parse(statusJson);
                    string? jobName = doc.RootElement.GetProperty("jobname").GetString();
                    string? jobId = doc.RootElement.GetProperty("jobid").GetString();

                    if (!string.IsNullOrEmpty(jobName) && !string.IsNullOrEmpty(jobId))
                    {
                        var spoolFiles = await _jobService.GetJobSpoolFilesAsync(jobName, jobId, cancellationToken).ConfigureAwait(false);
                        foreach (var sf in spoolFiles)
                        {
                            if (sf != null && sf.Id.HasValue && !string.IsNullOrEmpty(sf.DdName) &&
                                (sf.DdName.Equals("SYSTSPRT", StringComparison.OrdinalIgnoreCase) ||
                                 sf.DdName.Equals("SYSPRINT", StringComparison.OrdinalIgnoreCase)))
                            {
                                return await _jobService.GetSpoolFileContentAsync(jobName, jobId, sf.Id.Value.ToString(), cancellationToken).ConfigureAwait(false);
                            }
                        }
                    }
                }
                catch
                {
                    return statusJson;
                }

                return statusJson;
            }

            return string.Empty;
        }

        public async Task<List<Db2TableItem>> ListUserTablesAsync(string creator, string? jobCard = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(creator))
            {
                throw new ArgumentException("Creator cannot be empty.", nameof(creator));
            }

            string sql = $"SELECT NAME AS TABLE_NAME, CREATOR, TYPE FROM SYSIBM.SYSTABLES WHERE CREATOR = '{creator.Trim().ToUpper()}';";
            string output = await ExecuteSqlViaJclAsync(sql, jobCard, cancellationToken).ConfigureAwait(false);

            var tables = new List<Db2TableItem>();
            if (string.IsNullOrWhiteSpace(output))
                return tables;

            var lines = output.Split('\n');
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("NAME") || trimmed.StartsWith("-") || trimmed.StartsWith("DSN") || trimmed.StartsWith("PAGE"))
                    continue;

                var parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3)
                {
                    tables.Add(new Db2TableItem
                    {
                        TableName = parts[0],
                        Creator = parts[1],
                        Type = parts[2]
                    });
                }
            }

            return tables;
        }
    }
}