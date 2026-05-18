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
    
        public async Task<string> RetrieveDatasetContentAsync(string datasetName,string memberName, RetrieveContentOptions options = null, CancellationToken cancellationToken = default)
        {
            if(string.IsNullOrEmpty(datasetName))
            {
                throw new ArgumentException("Dataset name cannot be empty", nameof(datasetName));
            }
            //GET /zosmf/restfiles/ds/[-(<volser>)/]<dataset-name>[(<member-name>)]
            options ??= new RetrieveContentOptions();
            if(!string.IsNullOrEmpty(options.Search) && !string.IsNullOrEmpty(options.Research))
            {
                throw new ArgumentException("Search and research parameters cannot be specified together");
            }
            var endpointBuilder = new StringBuilder($"/zosmf/restfiles/ds/}{Uri.EscapeDataString(datasetName)}");
            if(!string.IsNullOrWhiteSpace(datasetName))
            {
                endpointBuilder.Append($"{Uri.EscapeDataString(memberName)}");
            }

            //query parametresini ekleme

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(options.Search)) queryParams.Add($"search={Uri.EscapeDataString(options.Search)}");
            if (!string.IsNullOrEmpty(options.Research)) queryParams.Add($"search={Uri.EscapeDataString(options.Research)}");
            if (options.Insensitive.HasValue)
                queryParams.Add($"insensitive={options.Insensitive.Value.ToString().ToLower()}");

            if (options.MaxReturnSize.HasValue)
                queryParams.Add($"maxreturnsize={options.MaxReturnSize.Value}");

            if (queryParams.Count > 0)
            {
                endpointBuilder.Append("?").Append(string.Join("&", queryParams));
            }

            //X-IBM-Session-Ref gibi diğerleri eklenecek burdan sonrasında. 
            //Ronin'in bahsettiği sistemin .Net hali var mı ona bakılacak 


        }


    }
}
