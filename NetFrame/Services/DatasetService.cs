using Microsoft.Extensions.Logging;
using NetFrame.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class DatasetService : IDatasetService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DatasetService> _logger;

        public DatasetService(HttpClient httpClient, ILogger<DatasetService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<string>> ListDatasetsAsync(string dsLevel, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dsLevel))
            {
                throw new ArgumentException("Dataset level cannot be empty.", nameof(dsLevel));
            }

            var datasetNames = new List<string>();
            var endpoint = $"/zosmf/restfiles/ds?dslevel={Uri.EscapeDataString(dsLevel)}";

            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonNode>(endpoint, cancellationToken);

                if (response?["items"] is JsonArray itemsArray)
                {
                    foreach (var item in itemsArray)
                    {
                        string? name = item?["dsname"]?.ToString();
                        if (!string.IsNullOrEmpty(name))
                        {
                            datasetNames.Add(name);
                        }
                    }
                }

                return datasetNames;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing datasets for level: {DsLevel}", dsLevel);
                throw;
            }
        }



        public async Task CreateDatasetAsync(
            string datasetName,
            CreateDatasetRequest requestBody,
            CreateDatasetOptions options = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(datasetName))
            {
                throw new ArgumentException("dataset name cannot be empty", nameof(datasetName));
            }

            if (requestBody == null)
            {
                throw new ArgumentNullException(nameof(requestBody), "request body cannot be null when creating a dataset.");
            }

            options ??= new CreateDatasetOptions();

            var endpoint = $"/zosmf/restfiles/ds/{Uri.EscapeDataString(datasetName)}";

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

            request.Content = JsonContent.Create(requestBody);
            if (!string.IsNullOrEmpty(options.TargetSystem))
            {
                request.Headers.Add("X-IBM-Target-System", options.TargetSystem);
            }

            if (!string.IsNullOrEmpty(options.TargetSystemUser) && !string.IsNullOrEmpty(options.TargetSystemPassword))
            {
                request.Headers.Add("X-IBM-Target-System-User", options.TargetSystemUser);
                request.Headers.Add("X-IBM-Target-System-Password", options.TargetSystemPassword);
            }

            try
            {
                var response = await _httpClient.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error creating dataset: {DatasetName}", datasetName);
                throw;
            }
        }

        public async Task<DatasetMemberResponse> ListDatasetMembersAsync(string datasetName, ListMembersOptions? options = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(datasetName))
            {
                throw new ArgumentException("Dataset name cannot be empty.", nameof(datasetName));
            }

            options ??= new ListMembersOptions();

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(options.Start)) queryParams.Add($"start={Uri.EscapeDataString(options.Start)}");
            if (!string.IsNullOrEmpty(options.Pattern)) queryParams.Add($"pattern={Uri.EscapeDataString(options.Pattern)}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var endpoint = $"/zosmf/restfiles/ds/{Uri.EscapeDataString(datasetName)}/member{queryString}";

            using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

            if (options.MaxItems.HasValue)
            {
                request.Headers.Add("X-IBM-Max-Items", options.MaxItems.Value.ToString());
            }

            var attributes = options.Attributes;
            if (options.RequestTotalRows)
            {
                attributes += ",total";
            }
            request.Headers.Add("X-IBM-Attributes", attributes);


            if (!string.IsNullOrEmpty(options.MigratedRecall))
            {
                request.Headers.Add("X-IBM-Migrated-Recall", options.MigratedRecall);
            }

            try
            {
                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<DatasetMemberResponse>(cancellationToken: cancellationToken);
                return result ?? new DatasetMemberResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing members for dataset: {DatasetName}", datasetName);
                throw;
            }
        }

        public async Task<string> RetrieveDatasetContentAsync(
            string datasetName,
            string memberName = null,
            RetrieveContentOptions options = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(datasetName))
            {
                throw new ArgumentException("dataset name cannot be empty", nameof(datasetName));
            }

            options ??= new RetrieveContentOptions();

            if (!string.IsNullOrEmpty(options.Search) && !string.IsNullOrEmpty(options.Research))
            {
                throw new ArgumentException("search and research parameters cannot be specified together");
            }

            var endpointBuilder = new StringBuilder($"/zosmf/restfiles/ds/{Uri.EscapeDataString(datasetName)}");

            if (!string.IsNullOrWhiteSpace(memberName))
            {
                endpointBuilder.Append($"({Uri.EscapeDataString(memberName)})");
            }

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(options.Search))
                queryParams.Add($"search={Uri.EscapeDataString(options.Search)}");

            if (!string.IsNullOrEmpty(options.Research))
                queryParams.Add($"research={Uri.EscapeDataString(options.Research)}");

            if (options.Insensitive.HasValue)
                queryParams.Add($"insensitive={options.Insensitive.Value.ToString().ToLower()}");

            if (options.MaxReturnSize.HasValue)
                queryParams.Add($"maxreturnsize={options.MaxReturnSize.Value}");

            if (queryParams.Count > 0)
            {
                endpointBuilder.Append("?").Append(string.Join("&", queryParams));
            }


            //Ronin'in bahsettiği sistemin .Net hali var mı ona bakılacak 
            using var request = new HttpRequestMessage(HttpMethod.Get, endpointBuilder.ToString());
            if (!string.IsNullOrEmpty(options.IfNoneMatch) && string.IsNullOrEmpty(options.RecordRange))
            {
                request.Headers.Add("If-None-Match", options.IfNoneMatch);
            }


            if (!string.IsNullOrEmpty(options.DataType))
            {
                request.Headers.Add("X-IBM-Data-Type", options.DataType);
            }

            if (options.ReturnEtag.HasValue && options.ReturnEtag.Value && string.IsNullOrEmpty(options.RecordRange))
            {
                request.Headers.Add("X-IBM-Return-Etag", "true");
            }

            if (!string.IsNullOrEmpty(options.MigratedRecall))
            {
                request.Headers.Add("X-IBM-Migrated-Recall", options.MigratedRecall);
            }

            if (!string.IsNullOrEmpty(options.RecordRange))
            {
                request.Headers.Add("X-IBM-Record-Range", options.RecordRange);
            }

            if (!string.IsNullOrEmpty(options.ObtainEnq))
            {
                request.Headers.Add("X-IBM-Obtain-ENQ", options.ObtainEnq);
            }

            if (!string.IsNullOrEmpty(options.SessionRef))
            {
                request.Headers.Add("X-IBM-Session-Ref", options.SessionRef);
            }

            if (options.ReleaseEnq.HasValue && options.ReleaseEnq.Value)
            {
                request.Headers.Add("X-IBM-Release-ENQ", "true");
            }

            if (!string.IsNullOrEmpty(options.DsnameEncoding))
            {
                request.Headers.Add("X-IBM-Dsname-Encoding", options.DsnameEncoding);
            }


            if (!string.IsNullOrEmpty(options.TargetSystemUser) && !string.IsNullOrEmpty(options.TargetSystemPassword))
            {
                request.Headers.Add("X-IBM-Target-System-User", options.TargetSystemUser);
                request.Headers.Add("X-IBM-Target-System-Password", options.TargetSystemPassword);
            }

            try
            {
                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (response.StatusCode == System.Net.HttpStatusCode.NotModified)
                {
                    return null;
                }

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving content for dataset: {DatasetName}", datasetName);
                throw;
            }
        }



        //COBOL dosyasının içine string'den gelen kodu yazdırmak istiyorum mesela? 
        /*
                IDENTIFICATION DIVISION.
       PROGRAM-ID. HELLO.
       PROCEDURE DIVISION.
           DISPLAY 'HELLO TURKEY!'.
           GOBACK.
            



         */
        public async Task<string> WriteDatasetContentAsync(
            string datasetName,
            string memberName = null,
            string content = "",
            string volser = null,
            WriteContentOptions options = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(datasetName))
            {
                throw new ArgumentException("Dataset name cannot be empty", nameof(datasetName));
            }
            options ??= new WriteContentOptions();
            var endpointBuilder = new StringBuilder("/zosmf/restfiles/ds/");
            if (!string.IsNullOrWhiteSpace(volser))
            {
                endpointBuilder.Append($"-({Uri.EscapeDataString(volser)})/");
            }

            endpointBuilder.Append(Uri.EscapeDataString(datasetName));

            if (!string.IsNullOrWhiteSpace(memberName))
            {
                endpointBuilder.Append($"({Uri.EscapeDataString(memberName)})");
            }



            using var request = new HttpRequestMessage(HttpMethod.Put, endpointBuilder.ToString());
            var encoding = new UTF8Encoding(false); // hata atiyo mainframe'de 
            request.Content = new StringContent(content ?? string.Empty, encoding, options.ContentType);
            if (!string.IsNullOrEmpty(options.IfMatch))
            {
                request.Headers.TryAddWithoutValidation("If-Match", options.IfMatch);
            }



            //ibm headers 

            if (!string.IsNullOrEmpty(options.DataType))
            {
                request.Headers.Add("X-IBM-Data-Type", options.DataType);
            }

            if (!string.IsNullOrEmpty(options.MigratedRecall))
            {
                request.Headers.Add("X-IBM-Migrated-Recall", options.MigratedRecall);
            }

            if (!string.IsNullOrEmpty(options.ObtainEnq))
            {
                request.Headers.Add("X-IBM-Obtain-ENQ", options.ObtainEnq);
            }

            if (!string.IsNullOrEmpty(options.SessionRef))
            {
                request.Headers.Add("X-IBM-Session-Ref", options.SessionRef);
            }

            if (options.ReleaseEnq.HasValue && options.ReleaseEnq.Value)
            {
                request.Headers.Add("X-IBM-Release-ENQ", "true");
            }

            if (!string.IsNullOrEmpty(options.DsnameEncoding))
            {
                request.Headers.Add("X-IBM-Dsname-Encoding", options.DsnameEncoding);
            }

            if (!string.IsNullOrEmpty(options.TargetSystemUser) && !string.IsNullOrEmpty(options.TargetSystemPassword))
            {
                request.Headers.Add("X-IBM-Target-System-User", options.TargetSystemUser);
                request.Headers.Add("X-IBM-Target-System-Password", options.TargetSystemPassword);
            }


            try
            {

                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return response.Content.ToString();


            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "error writing content to dataset: {DatasetName}", datasetName);
                throw;
            }



        }
    }
}